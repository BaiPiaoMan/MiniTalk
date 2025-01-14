﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.Net;


/*                               《MiniTalk开发日志》
    * 开发人员：梅浩东
    * 当前版本（V1.0.2）
    * 起始构造时间：2019.11.8
    * 
    * 日志：
    * 2019.11.16：添加了鼠标移动到listbox每项上的特效,但刷新控件时会严重闪烁
    *             设置了用户窗口动态添加功能，实际效果还未测试
    *             详细添加了注释
    *             
    * 2019.11.17：详细设置了动态会话窗口生成和切换功能,并进行了初步测试
    *             设置了当前与哪个用户聊天的名称头像显示
    *             解决了窗体刷新时闪烁问题
    *             解决了群聊头像一直置顶的问题
    *             更改了启动LOGO颜色效果
    *             添加了创建用户界面自动根据ip地址选择默认频道的功能
    *             添加了创建用户界面显示本机ip地址的功能
    *             添加了创建用户界面频道帮助，以及鼠标移入按钮有动画的功能
    *             实现了退出程序，其他在线用户会将其从在线用户移除功能
    *             
    * 2019.11.18: 修复了下线时接收不到下线消息的问题
    *             修复了将发送的消息添加到会话窗口被listbox控件遮挡的问题
    *             修复了给对方发送消息，对方添加窗口卡住的问题
    *             修复了下线时如果正在该用户会话恢复默认聊天窗口时卡住的问题
    *             
    * 2019.11.19  降低了了下线后再重新上线时存在重新添加用户到在线列表偶尔失败的概率
    *             增加了异常信息处理端口的方法
    *             添加了鼠标移入控件后的提示信息
    *             
    * 2019.11.20  添加了用户消息暂存库，避免在添加消息时会使用户其他正在会话的窗口消息消失。在切换用户时将该用户的消息库中的消息添加到会话窗口中
    *                        
    * 2019.11.21  添加了全局异常捕捉功能，便于在发生未处理的异常时时进行善后操作
    *                   添加了用户点击发送框时会自动切换为搜狗输入法的功能
    *                   修复了创建会话控件时的坐标偏差问题
    *                   添加了用户更改频道时询问的功能(已废除)
    *                   添加了好友上下线提示
    *             
    * 2019.11.22  优化了网络收发数据的逻辑，大大降低了接收数据后格式紊乱导致的消息显示异常概率
    *                       
    * 2019.11.23  添加了用户收到消息时会红点标注
    * 
    * 2019.11.24  优化了头像加载算法
    * 
    * 2019.11.25  修复了输入框粘贴文本会引发异常的BUG
    * 
    * 2019.11.27  修复了消息泄露到私人会话窗口的BUG
    *                   优化了ip网段灵活自动切换的功能，由原本只能是网段1优化为只要是同网段都可以聊天
    *             
    * 2019.12.25  优化了消息发送机制，大大提升了发送消息的数据量，并且取消了禁止粘贴的模块.
    *                   发送消息时显示发送时间，消息结构更加清晰
    * 
    * 2019.12.28  添加了文件发送窗体，文件接收窗体以及连接用户预加载窗体，并将他们之间的逻辑进行了联系
    * 
    * 2020.1.3    (重大更新)成功添加了文件发送机制，但是大小只限于1G以下,有待提升;
    *                  添加了启动名称随机机制
    *                  添加了名称违规检测机制
    *                  添加了定时重发上线消息机制（已废除改为手动刷新）
    * 
    * 2020.1.4    优化了文件发送机制，由双方反复交替进行 发送->确认->发送（已废除改为分段发送读写）
    * 
    * 2020.1.5    着重优化了发送文件偶尔接收不全的问题,现情况大有改善,但仍存在,发现分段包大小=1024B时则不存在该情况,但文件发送缓慢
    * 
    * 2020.1.7    添加了文件发送中途能够取消的功能,优化了文件收发的代码结构，标签始终居中
    * 
    * 2020.1.21   添加了用户查看历史消息时不会将对话框强行拖至底部,涉及到滚动条边界判断，在网上下载了实例项目复制了其此逻辑代码。
    *                   添加了当用户查看历史消息时实时提示下方接收到的新消息数量
    *             
    * 2020.1.24   (重大更新)将原有的richtextbox会话框改为了Panel控件气泡聊天，而且支持显示头像
    *                  更改了聊天窗口切换模式，窗口切换窗口时不进行隐藏，而将活动窗口置顶到最前方
    * 
    * 2020.1.25   实现了头像选择功能
    * 
    * 2020.1.26   实现了最大消息数容纳限制，并优化了代码
    * 
    * 2020.1.27   添加了发送表情功能
    * 
    * 2020.1.30   优化了表情插入机制,并修复了表情占位符会被换行符阻断的Bug
    * 
    * 2020.1.31   添加了表情选择窗口
    * 
    * 2020.2.7    简化代码结构
    * 
    * 2020.2.8    给每个窗体添加了进入和退出的动画效果,并封装为一类(改为封装为DLL)
    * 
    * 2020.2.16   引用了了消息审判功能，如果发送的消息疑似存在侮辱我的情况则进行篡改
    * 
    * 2020.2.17   对程序功能进行了优化，并概念上的给出了任意频段通信的代码模型,实际效果待测试
    *                   优化了代码结构，耦合度进一步降低了
    *             
    * 2020.2.24   修改了用户的消息暂存库为字典类型，从而更好查找消息发送者的信息            
    *             
    * 2020.2.25   添加了"MiniTalk.RemoteControl"(MiniTalk.RemoteControl窗体)控制端基本模型
    *                    添加了"Prompt"(生存期限消息提示窗体)(配置为DLL)
    *             
    * 2020.2.27   新增输入区消息公告功能
    * 
    * 2020.3.17   进行了两台电脑的实际测验，修复了若干BUG,发现聊天气泡在分辨率不同的屏幕上生成的长度也不同
    *                   文件发送功能出现了异常，未来得及修复
    *             
    * 2020.3.20   添加了列表用户刷新的按钮，便于在用户之间偶尔出现未能添加到对方到列表时重新发送上线消息进行添加
    * 
    * 2020.3.28   完成了文件传输模块的优化,上传和下载速度显著提升
    *             
    * 2020.3.30   尝试将文件大小改为无限制，但是接收方内存消耗跟文件大小成正比，必须优化
    * 
    * 2020.3.31   完美解决了大文件传输问题和内存占用的问题，文件传输模块内存占用降低到稳定100MB以下
    *                    文件传输速度还取决于硬盘读写速度
    *                    解决了文件传输关闭时偶尔释放窗体时报错的Bug
    *                    优化了体验，文件传输前将文件扩展名设置为不可用避免用户打开，结束后还原
    *             
    * 2020.4.1    将Prompt类和Animation类封装为DLL引用文件
    * 
    * 2020.4.2    添加了设置模块，并设计了窗体
    *      
    * 2020.4.3    设置了禁止同时运行多个本程序的约束
    *                   添加了会话控件size属性大小过高处理
    *             
    * 2020.4.4    设置了消息框内滑动鼠标滑轮时联动父级滚动条
    * 
    * 2020.4.5    设置了设置窗口拖动功能
    *                  设置了消息选中时鼠标外界自动拖放功能
    *             
    * 2020.4.7    设置了设置属性的永久保存特性
    *                   优化了添加长文本到窗口的加载算法，提升了添加速度
    *             
    * 2020.4.9    设置了用户名在主窗口的显示，优化了设置属性
    * 
    * 2020.4.10   (重大优化)消息发送和接收机制由原有的字符串分割获取数据改为Json的序列化和反序列化获取数据，减少了线程数量
    *                   (重大优化)优化图像资源调用机制以及统调整了图片大小,内存占用由原来200MB+稳定到100MB-
    *             
    * 2020.5.3    在距上次期间研究出了MiniTalk.RemoteControl模块但未添加到程序中，还存在若干问题,性能问题是瓶颈,传输画面只能维持在5~8Fps/s
    *                  画面传输复杂界面时底部有几率无法加载完整图像，并且切换窗口解压流会报错，原因未知
    *                  优化了程序结构，维护更便捷
    *                  取消了暂存消息库
    *                  增加了若干头像
    *             
    *2020.5.20    完善了MiniTalk.RemoteControl主体模块,实际使用效果未测试
    *                    祛除了气泡毛边,但是导致主体和气泡尾部之间的衔接有黑边
    *             
    *2020.6.19    彻底完成了远程协助模块，最终采用了微软提供的RDP协议进行远程协助,舍弃了以前自己用
    *                    算法搭建的远程协助模块
    *             
    *2020.6.20    尝试更改文件传输的发起和接收机制，由原本的对话框式改为了窗口会话气泡式，
    *                   目前只实现了外观和内部插入机制
    *             
    *2020.6.24    完成了气泡式的文件传输基本功能，仍有些Bug需要修复
    *                   优化了程序主要界面，改善了外观
    * 
    *2020.6.25    添加了事件消息类型，并对为窗体支持随意大小缩放进行了改进
    * 
    *2020.6.26    修复了窗体缩放时内部气泡消息紊乱的BUG,以及完善了他们的缩放逻辑
    * 
    *2020.6.27    测试了文件传输功能，能实现多文件并发下载，但传输结束后一段时间会阻塞UI线程，目前已经找到原因
    * 
    *2020.6.28    添加了事件消息类的网络传输架构
    *             添加了图片查看器窗体
    *             
    *2020.7.1     添加了图片消息传输机制
    *             添加了输入框智能识别粘贴功能
    *             修复了文件传输之间的部分Bug
    *             
    *2020.7.3     优化了滚动消息代码逻辑
    *             更改了图片消息发送接受的内部代码
    *             
    *2020.7.9     (重大更新)通过udp分包重组等功能成功完成了udp图像消息基本功能，并有几率成功接收图像消息，因为丢包率太高,待优化
    * 
    *2020.7.11    通过包重发机制、CRC8数据校验、漏包检测等手段成功完善了图片消息功能
    * 
    *2020.7.15    重构了代码架构，可维护性提升了
    * 
    *2020.7.18    重新设计了程序主体风格
    * 
    *2020.7.26    修复了文件传输的主要BUG
    *                    完成了功能-IP备注
    *                    完成了功能-用户黑名单
    *                    完成了功能-用户置顶
    *    
    *2020.7.27    完成了功能-好友列表查找
    * 
    *2020.8.22    优化了代码结构
    * 
    * 2020.9.1      设置部分窗体的分辨率自适应
    *                    优化了远程协助主控端的快捷菜单
    *                    
    * 2020.9.2      添加了远程桌面兼容检测
    *                     修复了取消置顶引起的索引异常
    *                     修复了收到表情时引起的界面卡顿
    *                     添加了推广模块
    *                     
    * 2020.9.13    期间修复了若干BUG
    * 
    * 2020.9.26    添加了快速截图模块
    * 
    * 2020.9.30    修复了win7远程协助方面的Bug
    * 
    * 2020.10.5     修复若干Bug
    * 
    * 2020.10.7     删除了主界面背景
    *                     改变了表情窗口的显示方式
    * 
    * 2020.10.9    添加了桌面右下角队列悬浮消息提示
    *                     更改主窗口布局
    *                     新增隐藏/显示 会话栏功能
    *                     新增拖放到顶部自动隐藏功能
    *                     
    * 2020.10.10   美化了在线列表滚动条
    * 
    * 2020.10.11   修复了一些Bug
    * 
    * 2020.10.20   添加了@功能
    *  
    * 2020.10.15   修复了后台接受消息宽度坍塌的Bug
    * 
    * 待解决：     
    *             
    * 待添加：     
    * 
    * 待测试：      
    */

namespace MiniTalk
{
    public static class Program
    {
        const string MUTEXLOCK = "MiniTalkProcessLock";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var m=new Mutex(true,MUTEXLOCK))
            {
                //检查进程是否已经启动，已经启动则显示报错信息退出程序。 
                if (!m.WaitOne(100,false))
                {
                    MessageBox.Show("本程序已经在运行!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Application.Exit();
                    return;
                }

                //IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
                //IPAddress localIp = ips[ips.Length - 1];
                //string[] strs = localIp.ToString().Split('.');
                //if (strs[0] != "192")
                //{
                //    MessageBox.Show("检测到网卡非C类私有IP地址,因此无法使用本程序", "启动失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Landing());
            }
        }
    }
}

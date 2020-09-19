﻿/*
*
* 文件名    ：MainViewModel                             
* 程序说明  : 首页模块
* 更新时间  : 2020-05-12 21:50
* 联系作者  : QQ:779149549 
* 开发者群  : QQ群:874752819
* 邮件联系  : zhouhaogg789@outlook.com
* 视频教程  : https://space.bilibili.com/32497462
* 博客地址  : https://www.cnblogs.com/zh7791/
* 项目地址  : https://github.com/HenJigg/WPF-Xamarin-Blazor-Examples
* 项目说明  : 以上所有代码均属开源免费使用,禁止个人行为出售本项目源代码
*/

namespace Consumption.ViewModel
{
    using Consumption.Common.Contract;
    using Consumption.Core.Aop;
    using Consumption.Core.Interfaces;
    using Consumption.ViewModel.Common;
    using Consumption.ViewModel.Interfaces;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime;
    using System.Threading.Tasks;
    using Ubiety.Dns.Core.Records.NotUsed;

    /// <summary>
    /// 应用首页
    /// </summary>
    public class MainViewModel : BaseDialogViewModel, IBaseDialog
    {
        public MainViewModel()
        {
            OpenPageCommand = new RelayCommand<string>(pageName => Messenger.Default.Send(pageName, "OpenPage"));
            ClosePageCommand = new RelayCommand<string>(pageName => Messenger.Default.Send(pageName, "ClosePage"));
            GoHomeCommand = new RelayCommand(InitHomeView);
            ExpandMenuCommand = new RelayCommand(() =>
            {
                for (int i = 0; i < ModuleManager.ModuleGroups.Count; i++)
                {
                    var arg = ModuleManager.ModuleGroups[i];
                    arg.ContractionTemplate = !arg.ContractionTemplate;
                }
                Messenger.Default.Send("", "ExpandMenu");
            });
        }

        #region Property

        private ModuleUIComponent currentModule;

        /// <summary>
        /// 当前选中模块
        /// </summary>
        public ModuleUIComponent CurrentModule
        {
            get { return currentModule; }
            set { currentModule = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<ModuleUIComponent> moduleList;

        /// <summary>
        /// 所有展开的模块
        /// </summary>
        public ObservableCollection<ModuleUIComponent> ModuleList
        {
            get { return moduleList; }
            set { moduleList = value; RaisePropertyChanged(); }
        }

        private ModuleManager moduleManager;

        /// <summary>
        /// 模块管理器
        /// </summary>
        public ModuleManager ModuleManager
        {
            get { return moduleManager; }
            set { moduleManager = value; RaisePropertyChanged(); }
        }

        #endregion

        #region Command

        /// <summary>
        /// 菜单栏收缩
        /// </summary>
        public RelayCommand ExpandMenuCommand { get; private set; }

        /// <summary>
        /// 返回首页
        /// </summary>
        public RelayCommand GoHomeCommand { get; private set; }

        /// <summary>
        /// 打开新页面，string: 模块名称
        /// </summary>
        public RelayCommand<string> OpenPageCommand { get; private set; }

        /// <summary>
        /// 关闭选择页, string: 模块名称
        /// </summary>
        public RelayCommand<string> ClosePageCommand { get; private set; }

        public RelayCommand MinCommand { get; private set; } = new RelayCommand(() =>
        {
            Messenger.Default.Send("", "WindowMinimize");
        });

        public RelayCommand MaxCommand { get; private set; } = new RelayCommand(() =>
        {
            Messenger.Default.Send("", "WindowMaximize");
        });

        #endregion

        /// <summary>
        /// 初始化页面上下文内容
        /// </summary>
        /// <returns></returns>
        public async Task InitDefaultView()
        {
            /*
             *  加载首页的程序集模块
             *  1.首先获取本机的所有可用模块
             *  2.利用服务器验证,过滤掉不可用模块
             *
             *  注:理论上管理员应该可用本机的所有模块, 
             *  当检测本机用户属于管理员,则不向服务器验证
             */

            //创建模块管理器
            ModuleManager = new ModuleManager();
            ModuleList = new ObservableCollection<ModuleUIComponent>();
            //加载自身的程序集模块
            await ModuleManager.LoadAssemblyModule();
            InitHomeView();
        }

      
        /// <summary>
        /// 初始化首页
        /// </summary>
        void InitHomeView()
        {
            var dialog = NetCoreProvider.Get<IBaseModule>("HomeCenter");
            dialog.BindDefaultModel();
            ModuleUIComponent component = new ModuleUIComponent();
            component.Name = "首页";
            component.Body = dialog.GetView();
            ModuleList.Add(component);
            ModuleManager.Modules.Add(component);
            CurrentModule = ModuleList[ModuleList.Count - 1];
        }
    }
}

﻿using System.Drawing;
using H.NotifyIcon;
using H.NotifyIcon.Core;

namespace HLab.UserNotification.Wpf
{
    public class UserNotificationServiceWpf : UserNotificationService
    {
        public string ToolTipText
        {
            get => _toolTipText;
            set
            {
                _toolTipText = value;
                if (_taskbarIcon != null)
                    _taskbarIcon.ToolTipText = value;
            }
        }

        public Icon Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                if(_taskbarIcon!=null)
                    _taskbarIcon.Icon = value;
            }
        }

        TaskbarIcon _taskbarIcon;
        Icon _icon;
        string _toolTipText;

        public void Show()
        {
            _taskbarIcon = new TaskbarIcon
            {
                Icon = Icon,
                ToolTipText = ToolTipText,
                
            };

        }

        public void Notify(string title, string message)
        {
            _taskbarIcon.ShowNotification(title, message, NotificationIcon.None);
        }
        public void NotifyInfo(string title, string message)
        {
            _taskbarIcon.ShowNotification(title, message, NotificationIcon.Info);
        }
        public void NotifyWarning(string title, string message)
        {
            _taskbarIcon.ShowNotification(title, message, NotificationIcon.Warning);
        }
        public void NotifyError(string title, string message)
        {
            _taskbarIcon.ShowNotification(title, message, NotificationIcon.Error);
        }

    }
}

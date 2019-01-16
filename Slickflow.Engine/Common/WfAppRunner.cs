﻿/*
* Slickflow 开源项目遵循LGPL协议，也可联系作者获取企业版商业授权和技术支持；
* 除此之外的使用则视为不正当使用，请您务必避免由此带来的一切商业版权纠纷和损失。
*  
The Slickflow project.
Copyright (C) 2014  .NET Workflow Engine Library

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, you can access the official
web page about lgpl: https://www.gnu.org/licenses/lgpl.html
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slickflow.Engine.Delegate;

namespace Slickflow.Engine.Common
{
    /// <summary>
    /// 流程执行人(业务应用的办理者)
    /// </summary>
    public class WfAppRunner
    {
        #region 属性
        public string AppName { get; set; }
        public string AppInstanceID { get; set; }
        public string AppInstanceCode { get; set; }
        public string ProcessGUID { get; set; }
        public string ProcessCode { get; set; }
        public string Version { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }

        public Nullable<int> TaskID { get; set; }        //任务ID，区分当前用户ActivityInstance列表的唯一任务
        public IDictionary<string, string> Conditions = new Dictionary<string, string>();
        public IDictionary<string, string> DynamicVariables { get; set; }
        public IDictionary<string, PerformerList> NextActivityPerformers { get; set; }

        internal DelegateEventList DelegateEventList = new DelegateEventList();
        #endregion
    }

    /// <summary>
    /// 流程返签、撤销和退回接收人的实体对象
    /// </summary>
    public class WfBackwardTaskReceiver
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string ActivityName { get; set; }

        /// <summary>
        /// 构造WfBackwardReceiver实例
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="backwardToActivityName"></param>
        /// <returns></returns>
        public static WfBackwardTaskReceiver Instance(string backwardToActivityName,
            string userID,
            string userName)
        {
            var instance = new WfBackwardTaskReceiver();
            instance.ActivityName = backwardToActivityName;
            instance.UserID = userID;
            instance.UserName = userName;
            
            return instance;
        }
    }
}

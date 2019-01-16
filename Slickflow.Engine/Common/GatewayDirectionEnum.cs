﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slickflow.Engine.Common
{
    /// <summary>
    /// 流程分支路由选择
    /// </summary>
    public enum GatewayDirectionEnum : int
    {
        /// <summary>
        /// 未指定
        /// </summary>
        None = 0,

        /// <summary>
        /// 或分支
        /// </summary>
        OrSplit = 1,

        /// <summary>
        /// 异或分支
        /// </summary>
        XOrSplit = 2,

        /// <summary>
        /// 多实例或分支
        /// </summary>
        OrSplitMI = 3,

        /// <summary>
        /// 并行分支
        /// </summary>
        AndSplit = 4,

        /// <summary>
        /// 多实例并行分支
        /// </summary>
        AndSplitMI = 5,

        /// <summary>
        /// 复杂分支
        /// </summary>
        ComplexSplit = 8,

        /// <summary>
        /// 所有分支类型
        /// </summary>
        AllSplitType = 15,

        /// <summary>
        /// 或合并
        /// </summary>
        OrJoin = 16,

        /// <summary>
        /// 或合并(多实例)
        /// </summary>
        OrJoinMI = 17,

        /// <summary>
        /// 异或合并
        /// </summary>
        XOrJoin = 32,

        /// <summary>
        /// 并行合并
        /// </summary>
        AndJoin = 64,

        /// <summary>
        /// 与合并(多实例)
        /// </summary>
        AndJoinMI = 65,

        /// <summary>
        /// 复杂合并
        /// </summary>
        ComplexJoin = 128,

        /// <summary>
        /// 所有合并类型
        /// </summary>
        AllJoinType = 240
    }
}

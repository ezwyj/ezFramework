
using Right.Entity;
using Right.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace Common
{
    public class RightBehavior : IInterceptionBehavior
    {
        public RightBehavior()
        {

        } static ApplicationRightService rightService = new ApplicationRightService("ProjectArchiveConn");



        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }
        bool IInterceptionBehavior.WillExecute
        {
            get
            {
                return true;
            }
        }



        IMethodReturn IInterceptionBehavior.Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var mb = input.MethodBase;

            object[] attrObj = mb.GetCustomAttributes(typeof(RightAttribute), false);
            if (attrObj.Count() > 0)
            {
                string badge = "";
                for (int i = 0; i < input.Arguments.Count; i++)
                {
                    if (input.Arguments[i] != null)
                    {


                        string x = input.Arguments[i].GetType().ToString();
                        

                    }

                }

                var rightInfo = (RightAttribute)attrObj[0];

                User u = rightService.GetUserByBadge(badge);
                if (u == null)
                {
                    throw new InfoException("未对登录用户分配权限用户!");
                }
                List<Role> userRole = rightService.GetRoleByUser(u);
                if (userRole == null)
                {
                    throw new InfoException("未对用户分配角色权限!");
                }

                //查找当前用户的角色,判断权限 
                bool havePermission = rightService.HaveRight(badge, rightInfo.ResourceName, rightInfo.OperationCode);


                if (havePermission)
                {
                    var methodReturn = getNext().Invoke(input, getNext); //可调用执行方法
                    return methodReturn;
                }
                else
                {
                    throw new InfoException("没有权限!");
                }

            }
            else
            {
                var methodReturn = getNext().Invoke(input, getNext); //调用执行方法
                return methodReturn;
            }







        }


    }

}

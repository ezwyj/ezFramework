using Business.Entity;
using Business.Interface;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Service
{
    public class ValueSetService : IValueSet
    {
        private PetaPoco.Database db;
        public ValueSetService(PetaPoco.Database para)
        {
            db = para;
        }
        
        /// <summary>
        /// 创建值集
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ValueSet CreateValueSet(string name, string description, string userName, string badge)
        {
            string msg = string.Empty;
            var v = new ValueSet();
            v.Text = name;
            v.Description = description;
            v.Creator = badge;
            v.CreateTime = DateTime.Now;
            v.CreatorID = userName;
            v.IsDelete = false;
            v.IsEnabled = true;

            var duptest = db.Fetch<ValueSet>(" where Text=@0 ", name).Where(o => o.IsDelete == false).ToList();
            if (duptest.Count > 0)
            {
                throw new InfoException("同名的值集已经存在!");

            }

            try
            {
                db.Save(v);
            }
            catch (Exception ex)
            {
                throw new InfoException("创建失败。" + ex.Message + "!");
            }
            return v;
        }
        public ValueSet GetValueSetByName(string name)
        {
            var vs = db.Fetch<ValueSet>(" where text=@0 and isdelete = 0", name).FirstOrDefault();
            return vs;

        }
        public ValueSet GetValueSetByID(int id)
        {
            var vs = db.Fetch<ValueSet>(" where id=@0", id).Where(o => o.IsDelete == false).FirstOrDefault();
            return vs;

        }
        public ValueItem GetValueItemByID(int id)
        {
            var vs = db.Fetch<ValueItem>(" where Id=@0", id).Where(o => o.IsDelete == false).FirstOrDefault();
            return vs;

        }

        public void SaveValueSet(ValueSet v)
        {
            string msg;
            try
            {
                var dupval = db.FirstOrDefault<ValueSet>(" where id<>@0 and text=@1", v.Id, v.Text);
                if (dupval != null)
                {
                    throw new InfoException("同名的值集已经存在!");
                }
                if (v.Id == 0)
                {
                    v.IsDelete = false;
                    v.IsEnabled = true;
                    if (string.IsNullOrEmpty(v.Text))
                    {
                        v.Text = v.Value;
                    }
                    if (string.IsNullOrEmpty(v.Value))
                    {
                        v.Value = v.Text;
                    }

                    db.Insert(v);
                }
                else
                {
                    db.Save(v);
                }
            }
            catch (Exception e)
            {

                throw e;

            }
        }

        public ValueItem CreateItem(ValueItem it)
        {
            string msg = "";
           

            var rst = ValidateItem(it);
            if (rst != null)
            {
                msg = string.Join(",", rst);
                throw new InfoException("创建值集项目失败," + msg + "!");
            }
            try
            {
                db.Save(it);
            }
            catch (Exception ex)
            {

                throw new InfoException("创建值集项目失败!");


            }


            return it;
        }
        public List<string> ValidateItem(ValueItem rec)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(rec.Value))
            {
                errors.Add("项目的值不能为空。");

            }

            if (string.IsNullOrWhiteSpace(rec.Text))
            {
                errors.Add("项目文本不能为空。");

            }

            if (rec.SetId == 0)
            {
                errors.Add("值集ID不能为空。");
            }


            else
            {
                var vs = db.Fetch<ValueSet>("where isdelete=0 and id=@0", rec.SetId);
                if (vs == null)
                {
                    errors.Add("值集ID不正确。");
                }
            }

            if (rec.Id == 0)
            {
                var items = db.Fetch<ValueItem>("where SetId=@0", rec.SetId).Where(o => o.IsEnabled == false).ToList();


                var it = items.FirstOrDefault(o => o.Value == rec.Value);
                if (it != null)
                {
                    if (it.Id != rec.Id)
                    {
                        errors.Add("记录值已经存在。");
                    }
                }


            }
            if (errors.Count > 0)
            {
                return errors;

            }
            return null;

        }
        public void SaveValueItem(ValueItem it)
        {
            string msg = "";
            try
            {
                
                if (string.IsNullOrEmpty(it.Text))
                {
                    it.Text = it.Value;
                }
                if (string.IsNullOrEmpty(it.Value))
                {
                    it.Value = it.Text;
                }

                List<string> rst = ValidateItem(it);
                if (rst != null)
                {
                    msg = string.Join(",", rst);
                    throw new Exception("修改值集项目失败" + msg + "!");
                }
                db.Save(it);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool DeleteValueItem(int id)
        {
            string msg = "";
            var it = db.Single<ValueItem>(id);


            if (it == null)
            {
                throw new Exception("项目不存在!");
            }

            it.IsDelete = true;

            db.Save(it);
            
            return true;
        }

        public bool DeleteValueSet(int id)
        {
            string msg = "";
            var it =  db.Single<ValueSet>(id);


            if (it == null || it.IsDelete == true)
            {
                msg = "值集不存在。";
                return false;

            }
            it.IsDelete = true;

            db.Save(it);

            return true;
        }

        public  List<ValueItem> GetValueItemList(int valueSetId)
        {
            return  db.Fetch<ValueItem>("where SetId=@0", valueSetId)
                    .Where(o => o.IsDelete == false)
                    .OrderBy(o => o.Sort)
                    .ToList();
        }

    }
}

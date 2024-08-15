using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Util;
using System.ComponentModel;
using System.Text;

namespace Druck.LMZ.ER.BusinessPlugIn
{
    [Description("差旅报销插件（两面针）")]
    [Kingdee.BOS.Util.HotUpdate]
    public class Druck_LMZ_ER_ExpReimbursement_Travel_Plugin : AbstractBillPlugIn //继承列表插件的基类
    {
        public override void DataChanged(DataChangedEventArgs e)
        {
            base.DataChanged(e); string key = e.Field.Key;
            if (key.EqualsIgnoreCase("FCOLLEAGUESID"))
            {
                //修改本院出差人员更新出差人数
                DynamicObject obj = this.Model.DataObject;
                DynamicObjectCollection COLLEAGUESIDCollection = obj["FCOLLEAGUESID"] as DynamicObjectCollection;

                this.View.Model.SetValue("FPEOPLESL", COLLEAGUESIDCollection.Count);

                //报销明细分录
                var eCount = this.View.Model.GetEntryRowCount("FEntity");
                for (int i = 0; i < eCount; i++)
                {
                    this.View.Model.SetValue("FEPEOPLESL", COLLEAGUESIDCollection.Count, i);
                    this.View.InvokeFieldUpdateService("FEPEOPLESL", i);
                }
                //行程信息分录
                var sCount = this.View.Model.GetEntryRowCount("FScheduleEntity");
                for (int i = 0; i < sCount; i++)
                {
                    this.View.Model.SetValue("FSPEOPLESL", COLLEAGUESIDCollection.Count, i);
                    this.View.InvokeFieldUpdateService("FSPEOPLESL", i);
                }
            }
        }

        public override void AfterCreateNewEntryRow(CreateNewEntryEventArgs e)
        {
            base.AfterCreateNewEntryRow(e);

            //报销明细分录
            if (e.Entity.Key.EqualsIgnoreCase("FEntity"))
            {
                var FPEOPLESL = this.View.Model.GetValue("FPEOPLESL");
                this.View.Model.SetValue("FEPEOPLESL", FPEOPLESL, e.Row);
                this.View.InvokeFieldUpdateService("FEPEOPLESL", e.Row);
            }

            //行程信息分录
            if (e.Entity.Key.EqualsIgnoreCase("FScheduleEntity"))
            {
                var FPEOPLESL = this.View.Model.GetValue("FPEOPLESL");
                this.View.Model.SetValue("FSPEOPLESL", FPEOPLESL, e.Row);
                this.View.InvokeFieldUpdateService("FSPEOPLESL", e.Row);
            }
        }

        public override void AfterEntryBarItemClick(AfterBarItemClickEventArgs e)
        {
            base.AfterEntryBarItemClick(e);

            var key = e.BarItemKey;
            //生成补贴明细触发
            if (key.EqualsIgnoreCase("tbFillByStd"))
            {
                StringBuilder sb = new StringBuilder();

                //报销明细分录
                var eCount = this.View.Model.GetEntryRowCount("FEntity");
                for (int i = 0; i < eCount; i++)
                {
                    this.View.InvokeFieldUpdateService("FEPEOPLESL", i);
                }

                //行程信息分录数
                var sCount = this.View.Model.GetEntryRowCount("FScheduleEntity");
                for (int i = 0; i < sCount; i++)
                {
                    this.View.InvokeFieldUpdateService("F_PAEZ_BTJEHJ", i);//补贴金额合计
                    this.View.InvokeFieldUpdateService("FSPEOPLESL", i);//人数
                }
            }
        }

        public override void AfterDoOperation(AfterDoOperationEventArgs e)
        {
            base.AfterDoOperation(e);

            // 新增：6，修改：5，删除：3，保存：8，提交：9，撤销：87，审核：1，反审核：26   
            if (e.Operation.OperationId == 8)
            {
                //行程信息分录数
                var sCount = this.View.Model.GetEntryRowCount("FScheduleEntity");
                for (int i = 0; i < sCount; i++)
                {
                    this.View.InvokeFieldUpdateService("F_PAEZ_BTJEHJ", i);//补贴金额合计
                    this.View.InvokeFieldUpdateService("FSPEOPLESL", i);//人数
                }

            }
        }
    }
}

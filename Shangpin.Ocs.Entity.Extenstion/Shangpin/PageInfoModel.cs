using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Shangpin.Ocs.Entity.Extenstion.ShangPin
{
    

   /// <summary>
   /// 公共实体层
   /// </summary>
   public class PaginationInfoModel
   {
       #region 分页实体
 
       private int _pageSize=20;
       private int _currentPage;
       private string _condition;
       private string _conditionSearch;
       private int _totalCount;
       private int _totalPage;
       private string _orderBy;
       private string _field;
       private string _selectfield;

       private string _tablename=string.Empty;
       /// <summary>
       /// 显示字段值
       /// </summary>
       public string Field
       {
         get{return _field;}
           set{_field=value;}
       }
       /// <summary>
       /// 查询字段显示字段值
       /// </summary>
       public string SelectField
       {
           get { return _selectfield; }
           set { _selectfield = value; }
       }
       /// <summary>
       /// 排序字段
       /// </summary>
       public string OrderBy
       {
           get { return _orderBy; }
           set { _orderBy = value; }
       }
       /// <summary>
       /// 总记录数据
       /// </summary>
       public int TotalCount
       {
           get { return _totalCount; }
           set { _totalCount = value; }
       }
 
       /// <summary>
       /// 总页数
       /// </summary>
       public int TotalPage
       {
           get { return _totalPage; }
           set { _totalPage = value; }
       }
        
       

       /// <summary>
       /// 分页大小
       /// </summary>
       public int PageSize
       {
           get { return _pageSize; }
           set { _pageSize = value; }
       }
       /// <summary>
       /// 当前页码
       /// </summary>
       public int CurrentPage
       {
           get { return _currentPage; }
           set { _currentPage = value; }
       }
      
       /// <summary>
       /// URL地址栏的参数拼接地址
       /// </summary>
       public string Condition
       {
           get { return _condition; }
           set { _condition = value; }
       }
       /// <summary>
       /// URL地址栏的参数
       /// </summary>
       public string ConditionSearch
       {
           get { return _conditionSearch; }
           set { _conditionSearch = value; }
       }

       public string TableName
       {
           get { return _tablename; }
           set { _tablename = value; }
       }
       #endregion
   }
}

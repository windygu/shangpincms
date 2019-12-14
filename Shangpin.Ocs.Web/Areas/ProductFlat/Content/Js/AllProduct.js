/*************************************************************************
* 数据定义、参数初始化
*************************************************************************/
var productList = [];//产品集合
var brandList = [];//品牌集合
var colorList = [];//色系集合
var categoryList = [];//分类集合
var sortedProductInfo = [];//当前OCS分类下已排序商品信息
var productCount = 0;//当前产品的总数
var cstParaEmpty = "undefined";//常量，表示色系、分类等为空
var sortedIndexList = [];//加入排序索引集合
// 排序产品参数
function SortPara() {
    this.productNo = "";
    this.limitedPrice = "";
    this.brandNo = "";
    this.colorNo = "";
    this.categoryNo = "";
}
// 搜索条件参数
var searchPara = {
    "pageIndex": "1",
    "ocsCategoryNo": "A01",
    "ocsCategoryName": "女性",
    "brandNo": "",
    "subType": "",
    "colorNo": "",
    "dateShelf": "",
    "price": "",
    "stock": "",
    "rate": ""
};

// 页面开始
$(document).ready(function () {
    parseData();
    parseQueryPara();
    refreshSortedIndex();
    setPagePara();
    bindEvent();
    disableSortedItem();
});

// 绑定事件
function bindEvent() {
    $(".JoinToSort").bind("click", onJoinSortClick);
    $("#BtnReset").click(onResetClick);
    $("#BtnSearch").click(onSearchClick);
    $("#BtnAllSelect").bind("click", onAllSelectClick);
    $("#BtnMulityJoin").bind("click", onBtnMulityClick);
    $(".ColorNo").each(onColorNoEach);
    var radio = document.getElementsByName("shelfType");
    $(radio[0]).bind("change", onRadio1Change);
    $(radio[1]).bind("change", onRadio2Change);
    $(radio[0]).trigger("change");
}

// 解析地址栏查询参数
function parseQueryPara() {
    var query = document.location.search;
    if (query != "" && query != undefined) {
        var queryArr = query.split("&");
        for (var i = 0; i < queryArr.length; i++) {
            var arr = queryArr[i].split("=");
            for (var key in searchPara) {
                if (arr[0].toLowerCase() == key.toLowerCase()) {
                    searchPara[key] = arr[1];
                }
            }
        }
        var name = searchPara.ocsCategoryName;
        if (name != decodeURI(name)) {
            searchPara.ocsCategoryName = decodeURI(name);
        }
    }
}

// 替换掉JSON脏字符
// [ input : string ]   输入值
function replaceDirtyString(input) {
    var r = input;
    var reg;
    reg = /"""/mgi;
    r = r.replace(reg, '"');
    return r;
}

// 解析后台返回数据
function parseData() {
    try {
        productCount = parseInt($("#HiddenProductCount").text(), 10);
        productList = eval("(" + replaceDirtyString($("#HiddenProductList").text()) + ");");
        brandList = eval("(" + $("#HiddenBrandList").text() + ");");
        colorList = eval("(" + $("#HiddenColorList").text() + ");");
        categoryList = eval("(" + $("#HiddenCategoryList").text() + ");");
        sortedProductInfo = eval("(" + $("#HiddenSortedProductInfo").text() + ");");
    } catch (e) {
        alert("解析数据出错");
    }
}

// 设置页面参数
function setPagePara()
{
    $("#fontAllCount").text(productCount);
    $("#path").text(searchPara.ocsCategoryName + "商品列表");
    NoProduct();
    resetSelBrand();
    resetSelSubType();
    resetSelColor();
    if (searchPara.price != "") {
        var arr = searchPara.price.split("~");
        $("#TxtMinPrice").val(arr[0]);
        $("#TxtMaxPrice").val(arr[1]);
    }
    if (searchPara.stock != "") {
        var arr = searchPara.stock.split("~");
        $("#TxtMinStock").val(arr[0]);
        $("#TxtMaxStock").val(arr[1]);
    }
    if (searchPara.rate != "") {
        var arr = searchPara.rate.split("~");
        $("#TxtMinDiscountRate").val(arr[0]);
        $("#TxtMaxDiscountRate").val(arr[1]);
    }
    if (searchPara.dateShelf != "") {
        var radio = document.getElementsByName("shelfType");
        radio[1].checked = true;
        $("#ShelfDate").val(searchPara.dateShelf);
    }
}

// 重置品牌下拉选择框
function resetSelBrand()
{
    var html = '';
    html += '<option value="0">选择品牌</option>';
    for (var i = 0; i < brandList.length; i++) {
        var name = brandList[i]["BrandChName"] + "  (" + brandList[i]["BrandEnName"] + ")";
        var selected = '';
        if (brandList[i]["BrandNO"].toLowerCase() == searchPara.brandNo.toLowerCase()) {
            selected = ' selected="selected"';
        }
        html += '<option value="' + brandList[i].BrandNO + '"' + selected + '>' + name + '</option>';
    }
    $("#SelBrand").html(html);
}

// 重置子类别下拉选择框
function resetSelSubType() {
    var html = '';
    html += '<option value="0">选择子类别</option>';
    for (var i = 0; i < categoryList.length; i++) {
        var selected = '';
        if (categoryList[i]["CategoryNo"].toLowerCase() == searchPara.subType.toLowerCase()) {
            selected = ' selected="selected"';
        }
        html += '<option value="' + categoryList[i].CategoryNo + '"'+selected+'>' + categoryList[i].CateGoryName + '</option>';
    }
    $("#SelSubType").html(html);
}

// 重置子类别下拉选择框
function resetSelColor() {
    var html = '';
    html += '<option value="0">选择色系</option>';
    for (var i = 0; i < colorList.length; i++) {
        var selected = '';
        if (colorList[i]["ColorNO"].toLowerCase() == searchPara.colorNo.toLowerCase()) {
            selected = ' selected="selected"';
        }
        html += '<option value="' + colorList[i].ColorNo + '">' + colorList[i].ColorName + '</option>';
    }
    $("#SelColor").html(html);
}

// 重置搜索表单
function resetSearchForm() {
    document.getElementById("SelBrand").selectedIndex = 0;
    document.getElementById("SelSubType").selectedIndex = 0;
    document.getElementById("SelColor").selectedIndex = 0;
    $("#TxtMinPrice").val("");
    $("#TxtMaxPrice").val("");
    $("#TxtMaxStock").val("");
    $("#TxtMinStock").val("");
    $("#TxtMaxDiscountRate").val("");
    $("#TxtMinDiscountRate").val("");
    $("#ShelfDate").val("");
    var radio = document.getElementsByName("shelfType");
    radio[0].checked = true;
    showDateShelf(false);
}

// 验证正整数
// [ input : string ]   验证值
// [ return : bool ]    成功与否
function validateNumber(input)
{
    //alert(input);
    var b = false;
    var reg = /^\d+$/gi;
    b = reg.test(input);
    return b;
}

// 验证浮点数
// [ input : string ]   验证值
// [ return : bool ]    成功与否
function validateFloat(input)
{
    //alert(input);
    var b = true;
    var reg = /^\d+(\.\d{1,2})?$/;
    b = reg.test(input);
    return b;
}

// 验证折扣
// [ input : string ]   验证值
// [ return : bool ]    成功与否
function validateRate(input) {
    var b = false;
    if (validateFloat(input)) {
        var f = parseFloat(input, 10);
        if (f < 10) {
            b = true;
        }
    }
    return b;
}

// 验证搜索表单
// [ return : bool ]    是否可以提交 
function validateSearchForm()
{
    var b = true;
    if ($("#SelBrand").val() != "0") {
        searchPara.brandNo = $("#SelBrand").val();
    }
    else {
        searchPara.brandNo = "";
    }
    if ($("#SelSubType").val() != "0") {
        searchPara.subType = $("#SelSubType").val();
    }
    else {
        searchPara.subType = "";
    }
    if ($("#SelColor").val() != "0") {
        searchPara.colorNo = $("#SelColor").val();
    }
    else {
        searchPara.colorNo = "";
    }
    var radio = document.getElementsByName("shelfType");
    if (radio[0].checked) {
        searchPara.dateShelf = "";
    }
    else {
        if ($("#ShelfDate").val() != "") {
            searchPara.dateShelf = $("#ShelfDate").val();
        }
        else {
            b = false;
            searchPara.dateShelf = "";
            alert("请选择上架时间");
        }
    }
    if ($("#TxtMinPrice").val() == "" && $("#TxtMaxPrice").val() == "") {
        searchPara.price = "";
    }
    else {
        if ($("#TxtMinPrice").val() != "" && $("#TxtMaxPrice").val() != "") {
            if (validateFloat($("#TxtMinPrice").val()) && validateFloat($("#TxtMaxPrice").val())) {
                var max = parseFloat($("#TxtMaxPrice").val(),10);
                var min = parseFloat($("#TxtMinPrice").val(),10);
                if (max > min) {
                    searchPara.price = $("#TxtMinPrice").val() + "~" + $("#TxtMaxPrice").val();
                }
                else {
                    b = false;
                    alert("价格范围输入错误");
                }
            }
            else {
                b = false;
                alert("请输入正确格式的价格, 最多保留两位小数");
            }
        }
        else {
            b = false;
            alert("请输入完整的价格范围");
        }
    }
    if ($("#TxtMinStock").val() == "" && $("#TxtMaxStock").val() == "") {
        searchPara.stock = "";
    }
    else {
        if ($("#TxtMinStock").val() != "" && $("#TxtMaxStock").val() != "") {
            if (validateNumber($("#TxtMinStock").val()) && validateNumber($("#TxtMaxStock").val())) {
                var max = parseInt($("#TxtMaxStock").val(), 10);
                var min = parseInt($("#TxtMinStock").val(), 10);
                if (max > min) {
                    searchPara.stock = $("#TxtMinStock").val() + "~" + $("#TxtMaxStock").val();
                }
                else {
                    b = false;
                    alert("库存范围输入错误");
                }
            }
            else {
                b = false;
                alert("请输入正确格式的库存");
            }
        }
        else {
            b = false;
            alert("请输入完整的库存范围");
        }
    }
    if ($("#TxtMaxDiscountRate").val() == "" && $("#TxtMinDiscountRate").val() == "") {
        searchPara.rate = "";
    }
    else {
        if ($("#TxtMaxDiscountRate").val() != "" && $("#TxtMinDiscountRate").val() != "") {
            if (validateRate($("#TxtMaxDiscountRate").val()) && validateRate($("#TxtMinDiscountRate").val())) {
                var max = parseFloat($("#TxtMaxDiscountRate").val(), 10);
                var min = parseFloat($("#TxtMinDiscountRate").val(), 10);
                if (max > min) {
                    searchPara.rate = $("#TxtMinDiscountRate").val() + "~" + $("#TxtMaxDiscountRate").val();
                }
                else {
                    b = false;
                    alert("折扣范围输入错误");
                }
            }
            else {
                b = false;
                alert("请输入正确格式的折扣, 最多保留两位小数");
            }
        }
        else {
            b = false;
            alert("请输入完整的折扣范围");
        }
    }
    return b;
}

// 获取搜索参数
// [ return : string ]  
function getSearchData()
{
    var data = "";
    data += "?pageindex=" + searchPara.pageIndex + "";
    data += "&OCSCategoryNo=" + searchPara.ocsCategoryNo + "";
    data += "&OCSCategoryName=" + encodeURI(searchPara.ocsCategoryName) + "";
    if (searchPara.brandNo != "") {
        data += "&BrandNo=" + searchPara.brandNo + "";
    }
    if (searchPara.colorNo != "") {
        data += "&ColorNo=" + searchPara.colorNo + "";
    }
    if (searchPara.dateShelf != "") {
        data += "&DateShelf=" + searchPara.dateShelf + "";
    }
    if (searchPara.price != "") {
        data += "&Price=" + searchPara.price + "";
    }
    if (searchPara.rate != "") {
        data += "&Rate=" + searchPara.rate + "";
    }
    if (searchPara.stock != "") {
        data += "&Stock=" + searchPara.stock + "";
    }
    if (searchPara.subType != "") {
        data += "&SubType=" + searchPara.subType + "";
    }
    return data;
}

// 显示或隐藏上架时间
// [ show : bool ]  显示与否
function showDateShelf(show)
{
    if (show) {
        $("#ShelfDate").css({"visibility":"visible"});
    }
    else {
        $("#ShelfDate").css({ "visibility": "hidden" });
    }
}

// 获取指定数据行的排序参数
// [ index : number ]   数据列表数据行索引
// [ return : object ]  排序参数
function getSortParaByIndex(index) {
    var r = new SortPara();
    r.productNo = productList[index]["ProductNo"];
    r.limitedPrice = productList[index]["LimitedPrice"];
    r.brandNo = productList[index]["BrandNo"];
    var colorNo = productList[index]["ProductPrimaryColor"];
    if (colorNo.indexOf(",") > -1) {
        colorNo = cstParaEmpty;//如果有多个色系，则置空
    }
    r.colorNo = colorNo;
    var categoryNo = productList[index]["Category"];
    if (categoryNo.indexOf(",") > -1) {
        categoryNo = cstParaEmpty;
    }
    r.categoryNo = categoryNo;
    return r;
}

// 获取色系信息
// [ colorNo : string ]     色系编号
// [ return : object ]      色系信息
function getColorInfoByColorNo(colorNo)
{
    for (var i = 0; i < colorList.length; i++) {
        if (colorList[i]["ColorNO"].toLowerCase() == colorNo.toLowerCase()) {
            return colorList[i];
            break;
        }
    }
}

// 获取品牌信息
// [ brandNo : string ]     品牌编号
// [ return : object ]      品牌信息
function getBrandInfoByBrandNo(brandNo) {
    for (var i = 0; i < brandList.length; i++) {
        if (brandList[i]["BrandNo"].toLowerCase() == brandNo.toLowerCase()) {
            return brandList[i];
            break;
        }
    }
}

// 获取色系名称
// [ colorNo : string ]     色系编号,可以多个,逗号分隔
// [ return : string ]      色系名称,可以多个,逗号分隔
function getColorName(colorNo)
{
    var colorName = "";
    if (colorNo != "" && colorNo != null) {
        var arr = colorNo.split(",");
        for (var i = 0; i < arr.length; i++) {
            //获取单个色系信息
            var color = getColorInfoByColorNo(arr[i]);
            colorName += "," + color["ColorName"];
        }
    }
    if (colorName != "") {
        colorName = colorName.substring(1);
    }
    return colorName;
}

// 显示没有查找到产品提示信息
function NoProduct()
{
    if (productList.length == 0) {
        $("#NoProduct").css({ "display": "block" });
        $("#ProductList").css({ "display": "none" });
        $("#ChkCtrl").css({ "display": "none" });
        $(".spPage").css({ "display": "none" });
    }
    else {
        $("#NoProduct").css({ "display": "none" });
        $("#ProductList").css({ "display": "block" });
        $("#ChkCtrl").css({ "display": "block" });
        $(".spPage").css({ "display": "block" });
    }
}

// 禁用已排序项
function disableSortedItem()
{
    var list = $(".JoinToSort");
    for (var i = 0; i < sortedIndexList.length ; i++) {
        list.eq(sortedIndexList[i]).attr("disabled", "true").val("已加入排序").unbind();
        $(".ChkProductItem").eq(sortedIndexList[i]).attr("disabled", "true").unbind();
    }
    //当前页全部加入排序
    if (sortedIndexList.length == 10) {
        $("#BtnAllSelect").css({ "visibility": "hidden" });
        $("#BtnMulityJoin").css({ "visibility": "hidden" });
    }
}

// 刷新已加入排序的产品的索引集合
function refreshSortedIndex()
{
    sortedIndexList = [];
    //遍历产品列表
    for (var i = 0; i < productList.length; i++) {
        for (var j = 0; j < sortedProductInfo.length; j++) {
            //检查它是否已排序
            if (productList[i]["ProductNo"].toLowerCase() == sortedProductInfo[j]["ProductNo"].toLowerCase()) {
                sortedIndexList.push(i);
                break;
            }
        }
    }
}

// 设置产品列表选中状态
// [ index : number ]    需要选中项的索引
// [ checked : bool ]    是否选中  
function setCheckboxStatus(index, checked) {
    if (checked) {
        //检查该项是否已加入排序
        var exist = false;
        for (var i = 0; i < sortedIndexList.length; i++) {
            if (index == sortedIndexList[i]) {
                exist = true;
                break;
            }
        }
        if (!exist) {
            $(".ChkProductItem").eq(index).attr("checked", "checked");
        }
    }
    else {
        $(".ChkProductItem").eq(index).removeAttr("checked");
    }
}

// 反解析加入排序数据
// [ list : object ]    SortPara数据集合
// [ return : string ]  字符窜
function unParseData(list)
{
    var r = "";
    var productNo = "";
    var brandNo = "";
    var colorNo = "";
    var categoryNo = "";
    var limitedPrice = "";
    for (var i = 0; i < list.length; i++) {
        productNo += ","+list[i]["productNo"];
        brandNo += "," + list[i]["brandNo"];
        colorNo += "," + list[i]["colorNo"];
        categoryNo += "," + list[i]["categoryNo"];
        limitedPrice += "," + list[i]["limitedPrice"];
    }
    r += "productNo=" + productNo.substring(1) + "&";
    r += "brandNo=" + brandNo.substring(1) + "&";
    r += "colorNo=" + colorNo.substring(1) + "&";
    r += "categoryNo=" + categoryNo.substring(1) + "&";
    r += "limitedPrice=" + limitedPrice.substring(1) + "";
    return r;
}

// 获取当前所有选中项排序参数集合
// [ list : object ]    排序参数集合
function getCheckedSortPara()
{
    var list = [];
    var indexList = getCheckedList();
    for (var i = 0; i < indexList.length; i++) {
        list.push(getSortParaByIndex(indexList[i]));
    }
    return list;
}

// 获取所有选中项索引
// [ list : object ]    索引集合
function getCheckedList()
{
    var list = [];
    for (var i = 0; i < $(".ChkProductItem").length; i++) {
        if ($(".ChkProductItem").eq(i).get(0).checked) {
            list.push(i);
        }
    }
    return list;
}

// 提交加入排序请求
function submitJoinToSort() {
    var list = getCheckedSortPara();
    if (list.length == 0) {
        return;
    }
    var data = unParseData(list);
    data += "&OCSCategoryNo=" + searchPara.ocsCategoryNo + "";
    data += "&OCSCategoryName=" + searchPara.ocsCategoryName + "";
    $.ajax({
        "type": "post",
        "url": "/ProductFlat/Product/DoJoinToSort",
        "datatype": "json",
        "data": data,
        "error": function () {
            alert("加入排序失败,网络可能发生故障或服务端错误");
            $(".ChkProductItem").removeAttr("checked");
        },
        "success": function (data) {
            if (data.result == "0") {
                alert(data.message);
                var indexList = getCheckedList();
                //加入新排序的索引
                for (var i = 0; i < indexList.length; i++) {
                    sortedIndexList.push(indexList[i]);
                }
                disableSortedItem();
                $(".ChkProductItem").removeAttr("checked");
            }
            else {
                alert(data.message);
            }
        }
    });
}

// 提交搜索请求
function submitSearch()
{
    if (validateSearchForm()) {
        var url = "/ProductFlat/Product/Index" + getSearchData();
        //alert(url);
        document.location = url;
    }
}



/*************************************************************************
* 事件处理
**************************************************************************/
// 加入排序按钮单击事件处理
function onJoinSortClick() {
    $(this).parent().parent().find(".ChkProductItem").eq(0).get(0).checked = true;
    submitJoinToSort();
}

// 重置按钮单击事件处理
function onResetClick() {
    resetSearchForm();
}

// 搜索按钮单击事件处理
function onSearchClick() {
    submitSearch();
}

// 全选按钮改单击事件处理
function onAllSelectClick() {
    var checked = false;
    var status = $(this).attr("status");
    if (status != undefined && status == "checked") {
        checked = true;
    }
    if (!checked) {
        for (var i = 0; i < productList.length; i++) {
            setCheckboxStatus(i, true);
        }
        $(this).attr("status", "checked");
    }
    else {
        $(".ChkProductItem").removeAttr("checked");
        $(this).attr("status", "nochecked")
    }
}

// 批量加入排序按钮单击事件处理
function onBtnMulityClick() {
    var indexList = getCheckedList();
    if (indexList.length > 0) {
        submitJoinToSort();
    }
    else {
        alert("请选择要加入排序的产品");
    }
}

// 单选框改变事件处理
function onRadio1Change()
{
    if (this.checked) {
        showDateShelf(false);
    }
}

// 单选框改变事件处理
function onRadio2Change() {
    if (this.checked) {
        showDateShelf(true);
    }
}

// 遍历色系编号
function onColorNoEach(index)
{
    var colorName = getColorName(productList[index]["ProductPrimaryColor"]);
    $(this).text(colorName);
}
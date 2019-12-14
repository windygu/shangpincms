/***********************************************************
* 参数定义
/**********************************************************/
var EGeneral = {    //是否常规项
    "yes": 1,
    "no": 0
};



$(document).ready(function () {
    initPage();
    bindEvent();
});

// 初始化页面数据
function initPage()
{
    var labelName = "";
    var query = document.location.search;
    if (query != "" && query != undefined) {
        query = query.substring(1);
        var arr = query.split("&");
        for (var i = 0; i < arr.length; i++) {
            var arr1 = arr[i].split("=");
            if (arr1[0].toLowerCase() == "labelname") {
                labelName = arr1[1];
                break;
            }
        }
    }
    $("#TxtSearchName").val(decodeURI(labelName));
}

// 绑定事件
function bindEvent() {
    $("#BtnSearchName").bind("click",onSearchClick);
    $("#AddLabelType").bind("click", onAddLabelTypeClick);
    $(".LabelAdd").each(function (index) {
        $(this).bind("click", onSndLabelAddClick);
        if (index % 2 == 0) {
            $(".ProductLabelListItem").eq(index).css({ "background-color": "#cfcfcf" });
        }
    });
    $(".EditLabelFst").each(function () {
        $(this).bind("click",onFstLabelEditClick);
    });
    //$(".DelLabelFst").each(function () {
    //    $(this).bind("click", onFstLabelDelClick);
    //});
    $(".EditSndLabel").each(function () {
        $(this).bind("click", onSndLabelEditClick);
    });
    //$(".DelSndLabel").each(function () {
    //    $(this).bind("click", onSndLabelDelClick);
    //});
}

// 创建添加标签分类窗口
function createAddLabelTypeHtml()
{
    var html = '';
    html += '<div id="">';
}

// 验证名称
// [ name : string ]    名称
// [ return : bool ]    是否合法
function validateName(name)
{
    var b = false;
    //var reg = /^([^\x00-\xFF]|[0-9]|[a-z]|[A-Z]|&|@|\(|\)){1,30}$/gi;
    //b = reg.test(name);
    if (name != "" && name != undefined) {
        b = true;
    }
    return b;
}

// 提交添加标签
// [ parentNo : string ]    父类编号
// [ labelName : string ]   标签名称
// [ labelType : number ]   标签类型
function submitAddLabel(parentNo, labelName, labelType, labelNiceName)
{
    var url = "/shangpin/productlabel/AddLabel";
    url += "?parentNo=" + parentNo + "&";
    url += "labelName=" + encodeURI(labelName) + "&";
    url += "labelType=" + labelType + "&";
    url += "labelNiceName=" + labelNiceName + "";
    //alert(url);
    $.ajax({
        "url": url,
        "type": "get",
        "data": "json",
        "success": function (data) {
            if (data.code == 0) {
                document.location.reload();
            }
            else {
                $("#ErrorMsg").text(data.msg).css({ "visibility": "visible" });
            }
        },
        "error": function () {
            alert("提交失败,网络故障或后台出现错误");
        }
    });
}

// 提交修改标签请求
// [ labelID : number ]     ID 
// [ parentNo : string ]    父类编号
// [ labelName : string ]   标签名称
// [ labelType : number ]   标签类型
function submitEditLabel(labelID, parentNo, labelName, labelType, labelNiceName) {
    var url = "/shangpin/productlabel/EditeLabel";
    url += "?labelId=" + labelID + "&";
    url += "labelName=" + encodeURI(labelName) + "&";
    url += "parentNo=" + parentNo + "&";
    url += "labelType=" + labelType + "&";
    url += "labelNiceName=" + encodeURI(labelNiceName) + "";
    //alert(url);
    $.ajax({
        "url": url,
        "type": "get",
        "data": "json",
        "success": function (data) {
            if (data.code == 0) {
                document.location.reload();
            }
            else {
                $("#ErrorMsg").text(data.msg).css({ "visibility": "visible" });
            }
        },
        "error": function () {
            alert("提交失败,网络故障或后台出现错误");
        }
    });
} 

// 提交删除标签请求
// [ labelNo : string ]    标签编号
function submitDelLabel(labelNo) {
    var url = "/shangpin/productlabel/DeleteLabel";
    url += "?labelNo=" + labelNo + "";
    //alert(url);
    $.ajax({
        "url": url,
        "type": "get",
        "data": "json",
        "success": function (data) {
            if (data.code == 0) {
                document.location.reload();
            }
            else {
                $("#ErrorMsg").text(data.msg).css({ "visibility": "visible" });
            }
        },
        "error": function () {
            alert("提交失败,网络故障或后台出现错误");
        }
    });
}

/***********************************************************
* 事件处理
/**********************************************************/
// 添加标签分类按钮单击事件处理
function onAddLabelTypeClick()
{
    var html = '';
    html += '名称：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
    html += '<input type="text" id="TxtLabelTypeName" maxlength="40" /><br/><br/>';
    html += '别名：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
    html += '<input type="text" id="labelNiceName" maxlength="40" style="width:160px;"/>';
    html += '<br /><br />是否常规项: <input type="checkbox" id="ChkLabelType" />';
    html += '<div id="ErrorMsg"></div>';
    var d = art.dialog({
        "title": "添加标签类",
        "content": html,
        "lock": true,
        "opacity": "0.2",
        "width":300,
        "padding": "30px 30px 0px 30px",
        "ok": function () {
            var name = $.trim($("#TxtLabelTypeName").val());
            var niceName = $.trim($("#labelNiceName").val());
            if (validateName(name)) {
                var type = EGeneral.no;
                if (document.getElementById("ChkLabelType").checked) {
                    type = EGeneral.yes;
                }
                submitAddLabel("Root", encodeURIComponent(name), type, niceName);
            }
            else {
                $("#ErrorMsg").text("请输入正确格式的名称").css({ "visibility": "visible" });
            }
            return false;
        },
        "cancel": function () { }
    });
}

// 一级标签修改按钮单击事件处理
function onFstLabelEditClick() {
    var labelName = $(this).parent().attr("labelName");
    var labelNo = $(this).parent().attr("labelNo");
    var labelId = $(this).parent().attr("labelID");
    var labelType = $(this).parent().attr("labelType");
    var labelNiceName = $(this).parent().attr("labelNiceName");
    var html = '';
    html += '名称：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
    html += '<input type="text" id="TxtLabelTypeName" maxlength="40" /><br/><br/>';
    html += '别名：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
    html += '<input type="text" id="labelNiceName" maxlength="40" style="width:160px;"/>';
    var checked = '';
    if (labelType == EGeneral.yes) {
        checked = ' checked="checked"';
    }
    html += '<br /><br />是否常规项: <input type="checkbox" id="ChkLabelType"' + checked + ' />';
    html += '<div id="ErrorMsg"></div>';
    var d = art.dialog({
        "title": "修改标签类",
        "content": html,
        "lock": true,
        "opacity": "0.2",
        "width": 300,
        "padding": "30px 30px 0px 30px",
        "init": function () {
            $("#TxtLabelTypeName").val(labelName);
            $("#labelNiceName").val(labelNiceName);
        },
        "ok": function () {
            var name = $.trim($("#TxtLabelTypeName").val());
            var niceName = $.trim($("#labelNiceName").val());
            if (validateName(name)) {
                var type = EGeneral.no;
                if (document.getElementById("ChkLabelType").checked) {
                    type = EGeneral.yes;
                }
                submitEditLabel(labelId, "Root", encodeURIComponent(name), type, niceName);
            }
            else {
                $("#ErrorMsg").text("请输入正确格式的名称").css({ "visibility": "visible" });
            }
            return false;
        },
        "cancel": function () { }
    });
}

// 一级标签删除按钮单击事件处理
function onFstLabelDelClick()
{
    var b = window.confirm("删除一级标签将删除该标签下所有标签, 确定要删除吗？");
    if (b) {
        submitDelLabel($(this).parent().attr("labelNo"));
    }
}

// 二级标签添加按钮单击事件处理
function onSndLabelAddClick()
{
    var parentNo = $(this).attr("labelno");
    var html = '';
    html += '名称：';
    html += '<input type="text" id="TxtLabelTypeName" maxlength="40" />';
    html += '<div id="ErrorMsg"></div>';
    var d = art.dialog({
        "title": "添加标签",
        "content": html,
        "lock": true,
        "opacity": "0.2",
        "width": 300,
        "padding": "30px 30px 0px 30px",
        "ok": function () {
            var name = $.trim($("#TxtLabelTypeName").val());
            if (validateName(name)) {
                submitAddLabel(parentNo, encodeURIComponent(name), 0,'');
            }
            else {
                $("#ErrorMsg").text("请输入正确格式的名称").css({ "visibility": "visible" });
            }
            return false;
        },
        "cancel": function () { }
    });
}

// 二级标签修改按钮单击事件处理
function onSndLabelEditClick()
{
    var parentNo = $(this).parent().attr("parentNo");
    var labelName = $(this).text();
    var labelID = $(this).parent().attr("labelid");
    var html = '';
    html += '名称：';
    html += '<input type="text" id="TxtLabelTypeName" maxlength="40" />';
    html += '<div id="ErrorMsg"></div>';
    var d = art.dialog({
        "title": "修改标签",
        "content": html,
        "lock": true,
        "opacity": "0.2",
        "width": 300,
        "padding": "30px 30px 0px 30px",
        "init": function () {
            $("#TxtLabelTypeName").val(labelName);
        },
        "ok": function () {
            var name = $.trim($("#TxtLabelTypeName").val());
            if (validateName(name)) {
                submitEditLabel(labelID, parentNo, encodeURIComponent(name), 0);
            }
            else {
                $("#ErrorMsg").text("请输入正确格式的名称").css({ "visibility": "visible" });
            }
            return false;
        },
        "cancel": function () { }
    });
}

// 二级标签删除按钮单击事件处理
function onSndLabelDelClick()
{
    var b = window.confirm("确定要删除吗？");
    if (b) {
        submitDelLabel($(this).parent().attr("labelno"));
    }
}

// 搜索按钮单击事件处理
function onSearchClick()
{
    var href = "/shangpin/productlabel/ProductLabelManager";
    var search = $.trim($("#TxtSearchName").val());
    if (search != "") {
        href += "?labelName=" + encodeURIComponent(search) + "";
    }
    document.location = href;
}
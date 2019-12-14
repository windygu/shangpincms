
//异步加载活动列表数据
function getActiveList(pageIndex) {
    $.ajax({
        type: "POST",
        url: "/ShangPin/Channel/GetActiveListJsonByPage",
        data: "TemplateNO=" + $("input[name='TemplateNO']").val() + "&relationID=" + $("input:hidden[name='RelationID']").val() + "&channelNO=" + $("input:hidden[name='ChannelNO']").val() + "&regionID=" + $("input[name='RegionID']").val() + "&pageIndex=" + pageIndex + "&pageSize=" + 5 + "&activeNameAndNO=" + $("#activeNameAndNO").val() + "&webSource=" + $("#webSource").val() + "&activeType=" + $("#activeType").val() + "&activeStatus=" + $("#activeStatus").val() + "&activeStartDate=" + $("#activeStartDate").val() + "&activeEndDate=" + $("#activeEndDate").val(),
        dataType: "json",
        success: function (data) {
            $("img[title='yulan']").attr("src", "/ReadPic/GetPic.ashx?width=100&height=100&pictureFileNo=" + data.relationobj.ImgNO + "&type=2");
            $("input:hidden[name='ImgNO']").val(data.relationobj.ImgNO);
            $("input:hidden[name='RelationID']").val(data.relationobj.RelationID);
            $("input:text[name='Description']").val(data.relationobj.Description);
            $("input:hidden[name='RelationType']").val(data.relationobj.RelationType);
            if (data.relationobj.RelationType == 5) {
                $("input:hidden[name='RelationContent']").val("");
            } else {
                $("input:hidden[name='RelationContent']").val(data.relationobj.RelationContent);
            }
            $("#activeList").empty(); //清空列表数据
            $.each(data.list, function (i, item) {
                $.tpl('activetemplate', {
                    'ActiveNO': item.ActiveNO,
                    'ActiveInfo': item.ActiveNO + "_" + item.WebSource + "_" + item.ActiveType,
                    'ActiveName': item.ActiveName.length > 5 ? item.ActiveName.substring(0, 5) : item.ActiveName,
                    'WebSource': item.WebSource == 1 ? '尚品' : '奥莱',
                    'ActiveType': item.ActiveType == 1 ? '活动' : '专题', 
                    'Status': item.Status == 1 ? '开启' : '关闭',
                    'ShowTime': item.ActiveStartTime + "至" + item.ActiveEndTime,
                    'Isable': item.ActiveID == -1 ? 'disabled' : item.Status == 1 ? '' : 'disabled'
                }).appendTo("#activeList");
            });
            $("#setpage").pager({ pagenumber: pageIndex, pagecount: data.count, buttonClickCallback: getActiveList });
        }
    });
}
//获取关联链接
function GetRelationLink() {
    $.ajax({
        type: "POST",
        url: "/ShangPin/Channel/GetRelationLinkJson",
        data: "TemplateNO=" + $("input[name='TemplateNO']").val() + "&relationID=" + $("input:hidden[name='RelationID']").val() + "&channelNO=" + $("input:hidden[name='ChannelNO']").val() + "&regionID=" + $("input[name='RegionID']").val(),
        dataType: "json",
        success: function (data) {
            $("#yulan").attr("src", "/ReadPic/GetPic.ashx?width=100&height=100&pictureFileNo=" + data.ImgNO + "&type=2");
            $("input:hidden[name='ImgNO']").val(data.ImgNO);
            $("input:hidden[name='RelationID']").val(data.RelationID);
            $("input:text[name='Description']").val(data.Description);
            $("input:hidden[name='RelationType']").val(data.RelationType);
            if (data.RelationType == 5) {
                $("input:text[name='linkurl']").val(data.RelationContent);
            } else {
                $("input:text[name='linkurl']").val("");
            }
        }
    });
}
var opratorobj = ""; //操作块对象
//单击弹出层编辑
function showEditeRegionDiv(obj) {
    opratorobj = $(obj);
    $("input:file[name='imgfile']").val("");
    $("input:hidden[name='RelationID']").val("0");
    $("input:hidden[name='RegionID']").val($(obj).attr("opratorRegion"));
    $("input:radio[name='selectedType'][value='" + $(obj).attr("relationtype") + "']").attr("checked", "checked");
    $("input:[name='RelationContent']").val("");
    $("input:[name='Description']").val("");
    if ($(obj).attr("relationtype") == "5") {
        $("input:radio[name='selectedType'][value='1']").attr("checked", "checked");
        $("input:radio[name='selectedType'][value='2']").removeAttr("checked");
        $("#activeEdite").hide();
        $("#linkEdite").show();
    } else {
        $("input:radio[name='selectedType'][value='1']").removeAttr("checked");
        $("input:radio[name='selectedType'][value='2']").attr("checked", "checked");
        $("#activeEdite").show();
        $("#linkEdite").hide();
    }
    $("body").append('<div id="windownBG" style="height:' + $(document).height() + 'px;opacity:0.5;z-index: 9999;display:block;left:0px;top:0px;position:absolute;width:100%;background: none repeat scroll 0 0 #000000;"></div>');
    $("#regionEdite").css({ 'top': ($(window).height() / 2) - 200 + $(window).scrollTop(), 'left': ($(window).width() / 2) - 410, 'display': 'block', 'z-index': '99999' });
    if ($(obj).attr("relationtype") == "5") {
        GetRelationLink();
        return;
    } else {
        getActiveList(1);
        return;
    }
}

//保存区块关联并且替换模板
function SaveRelationContent() {
    if ($("input[name='selectedType'][checked='checked']").val() == "2") {
        if ($("input[name='RelationContent']").val().length <= 0 && $("#activeList input:radio[name='dataobj'][checked='checked']").length <= 0) {
            alert("请选择关联活动");
            return;
        }
    }
    if ($("input[name='selectedType'][checked='checked']").val() == "1") {
        if ($("input[name='linkurl']").val().length <= 0) {
            alert("请填写连接地址");
            return;
        }
    }
    if ($("#ImgNO").val().length==0) {
        if ($("#imgfile").val().length == 0) {
            alert("请选择上传的图片");
            return;
        }
    }
    //异步提交表单
    $("#type2form").ajaxSubmit(function (data) {
        data = eval("(" + data + ")");
        if (data.status == 0) {
            alert(data.message);
            $("#regionEdite").hide();
            $('#windownBG').remove();
            return;
        }
        opratorobj.attr("relationtype", data.message.RelationType);
        opratorobj.empty();
        if (data.message.RelationContent.indexOf('#') == 0) {
            opratorobj.append('<img src="/ReadPic/GetPic.ashx?width=0&height=0&pictureFileNo=' + data.message.ImgNO + '&type=2"/>');
        } else {
            opratorobj.append('<a href="' + data.message.RelationContent + '"><img src="/ReadPic/GetPic.ashx?width=0&height=0&pictureFileNo=' + data.message.ImgNO + '&type=2""/></a>');
        }
        $('#windownBG').remove();
        $(".dispalayRegion").hide();
        $("#imgfile").val(""); //清空图片数据
        //重新绑定事件
        $("#loadTemplate a").click(function (event) {
            event.preventDefault();
        });

    });
}

//发布会场
function publishVenue() {
    if ($("#loadTemplate").text().length <= 0) {
        alert("会场内容不能为空");
        return;
    }
    $.post("/Shangpin/Channel/CreateHTMLByTemplate", { 'channelNO': $("#ChannelNO").val(), 'templateNO': $("#TemplateNO").val() }, function (htmldata) {
        $.post("/Shangpin/Channel/PublishVenue", { 'DetailID': $("#DetailID").val(), 'venuehtml': htmldata }, function (data) {
            if (data == "1") {
                alert("发布成功");
            } else {
                alert("发布失败" + data);
            }
        });
    });

}

function loadImg() {

    $("img[lazy]").each(function () {
        $(this).attr("src", $(this).attr("lazy")).removeAttr("lazy");
    });
}

$(document).ready(function () {
    loadImg();
    $.tpl('activetemplate', [
                '<tr><td><input name="dataobj" type="radio" value="{ActiveInfo:s}" {Isable:s}/></td><td>{ActiveName:s}</td><td>{ActiveType:s}</td><td>{WebSource:s}</td><td>{ActiveNO:s}</td><td>{Status:s}</td><td>{ShowTime:s}</td></tr>'
            ]);

    $("[relationtype]").bind("click", function () {
        showEditeRegionDiv(this);
    });
    $("#loadTemplate a").click(function (event) {
        event.preventDefault();
    });
    $("input:radio[name='selectedType']").click(function () {
        $("input:radio[name='selectedType']").attr("checked", false);
        $(this).attr("checked", "checked");
        $("#regionEdite").show();
        $("#regionEdite").css({ 'top': ($(window).height() / 2) - 200 + $(window).scrollTop(), 'left': ($(window).width() / 2) - 410, 'display': 'block', 'z-index': '99999' });
        if (this.value == "2") {
            $("#activeEdite").show();
            $("#linkEdite").hide();
            getActiveList(1);
            return;
        }
        if (this.value == "1") {
            $("#activeEdite").hide();
            $("#linkEdite").show();
            GetRelationLink();
            return;
        }
    });


});  
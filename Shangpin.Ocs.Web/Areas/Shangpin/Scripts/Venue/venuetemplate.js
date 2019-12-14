
//异步加载会场列表数据
function getVenueList(pageIndex) {
    $.ajax({
        type: "POST",
        url: "/ShangPin/Venue/GetVenueListJsonByPage",
        data: "TemplateNO=" + $("input[name='TemplateNO']").val() + "&MeetingRelationID=" + $("input:hidden[name='MeetingRelationID']").val() + "&venueID=" + $("#MeetingID").val() + "&venueNO=" + $("#MeetingNO").val() + "&mainVenueNO=" + $("#MainMeetingNO").val() + "&regionID=" + $("input[name='RegionID']").val() + "&pageIndex=" + pageIndex + "&nameAndDomain=" + $("#nameAndDomain").val() + "&activeNO=" + $("#activeNO").val() + "&status=" + $("#status").val() + "&startDate=" + $("#StartDate").val() + "&endDate=" + $("#EndDate").val(),
        dataType: "json",
        success: function (data) {
            $("img[title='yulan']").attr("src", "/ReadPic/GetPic.ashx?width=100&height=100&pictureFileNo=" + data.relationobj.ImgNO + "&type=2");
            $("input:hidden[name='ImgNO']").val(data.relationobj.ImgNO);
            $("input:hidden[name='MeetingRelationID']").val(data.relationobj.MeetingRelationID);
            $("#type1form input:[name='RelationContent']").val(data.relationobj.RelationContent);
            $("input:[name='OldRelationContent']").val(data.relationobj.RelationContent);
            $("input:text[name='Direction']").val(data.relationobj.Direction);
            $("input:hidden[name='oldRelationType']").val(data.relationobj.RelationType);
            $("#venueList").empty(); //清空列表数据
            $.each(data.list, function (i, item) {
                var doMainUrl = item.SourceFrom == 1 ? $("#ShangpinUrl").val() : $("#AoLaiUrl").val();
                $.tpl('template', {
                    'MeetingID': item.MeetingID,
                    'MeetingDomain': doMainUrl + item.MeetingDomain,
                    'IsSelect': item.MeetingID > 0 ? '' : 'checked=checked',
                    'MeetingName': item.MeetingName,
                    'Status': item.Status == 1 ? '开启' : '关闭',
                    'isAdd': (item.MeetingID == -1) ? 'disobj' : 'dataobj',
                    'ShowTime': item.WebPreViewCode + "至" + item.WebStartCode,
                    'Isable': item.MeetingID == -1 ? 'disabled' : ''
                }).appendTo("#venueList");
            });
            $("#setpage1").pager({ pagenumber: pageIndex, pagecount: data.count, buttonClickCallback: getVenueList });
        }
    });
}
//异步加载活动列表数据
function getActiveList(pageIndex) {
    $.ajax({
        type: "POST",
        url: "/ShangPin/Venue/GetActiveListJsonByPage",
        data: "TemplateNO=" + $("input[name='TemplateNO']").val() + "&MeetingRelationID=" + $("input:hidden[name='MeetingRelationID']").val() + "&venueID=" + $("#MeetingID").val() + "&venueNO=" + $("#MeetingNO").val() + "&mainVenueNO=" + $("#MainMeetingNO").val() + "&regionID=" + $("input[name='RegionID']").val() + "&pageIndex=" + pageIndex + "&activeNameAndNO=" + $("#activeNameAndNO").val() + "&webSource=" + $("#webSource").val() + "&activeType=" + $("#activeType").val() + "&activeStatus=" + $("#activeStatus").val() + "&activeStartDate=" + $("#activeStartDate").val() + "&activeEndDate=" + $("#activeEndDate").val(),
        dataType: "json",
        success: function (data) {
            $("img[title='yulan']").attr("src", "/ReadPic/GetPic.ashx?width=100&height=100&pictureFileNo=" + data.relationobj.ImgNO + "&type=2");
            $("input:hidden[name='ImgNO']").val(data.relationobj.ImgNO);
            $("input:hidden[name='MeetingRelationID']").val(data.relationobj.MeetingRelationID);
            $("#type2form input:[name='RelationContent']").val(data.relationobj.RelationContent);
            $("input:[name='OldRelationContent']").val(data.relationobj.RelationContent);
            $("input:text[name='Direction']").val(data.relationobj.Direction);
            $("input:hidden[name='oldRelationType']").val(data.relationobj.RelationType);
            $("#activeList").empty(); //清空列表数据
            $.each(data.list, function (i, item) {
                $.tpl('activetemplate', {
                    'ActiveID': item.ActiveID, 'ActiveNO': item.ActiveNO,
                    'ActiveName': item.ActiveName.length > 5 ? item.ActiveName.substring(0, 5) : item.ActiveName,
                    'IsSelect': item.ActiveID != -1 ? '' : 'checked=checked',
                    'WebSource': item.WebSource == 1 ? '尚品' : '奥莱',
                    'ActiveType': item.ActiveType == 1 ? '活动' : '专题', 'Status': item.Status == 1 ? '开启' : '关闭',
                    'isAdd': (item.ActiveID == -1) ? 'disobj' : 'dataobj',
                    'ShowTime': item.ActiveStartTime + "至" + item.ActiveEndTime,
                    'Isable': item.ActiveID == -1 ? 'disabled' : ''
                }).appendTo("#activeList");
            });
            $("#setpage2").pager({ pagenumber: pageIndex, pagecount: data.count, buttonClickCallback: getActiveList });
        }
    });
}
//获取关联链接
function GetRelationLink() {
    var mrId = $("input:hidden[name='MeetingRelationID']").val();
    var tNo = $("input[name='TemplateNO']").val();
    var meetingNo = $("#MeetingNO").val();
    var mainMeetingNo = $("#MainMeetingNO").val();
    var regionId = $("input[name='RegionID']").val();
    $.ajax({
        type: "POST",
        url: "/ShangPin/Venue/GetRelationLinkJson",
        data: "TemplateNO=" + tNo + "&MeetingRelationID=" + mrId + "&venueNO=" + meetingNo + "&mainVenueNO=" + mainMeetingNo + "&regionID=" + regionId,
        dataType: "json",
        success: function (data) {
            $("img[title='yulan']").attr("src", "/ReadPic/GetPic.ashx?width=100&height=100&pictureFileNo=" + data.ImgNO + "&type=2");
            $("input:hidden[name='ImgNO']").val(data.ImgNO);
            $("input:hidden[name='MeetingRelationID']").val(data.MeetingRelationID);
            $("#type3form input:[name='RelationContent']").val(data.RelationContent);
            $("input:text[name='Direction']").val(data.Direction);
            $("input:hidden[name='oldRelationType']").val(data.RelationType);
            if ($("input:hidden[name='oldRelationType']").val() != "3") {
                $("#type3form input:[name='RelationContent']").val("");
            }
        }
    });
}
var opratorobj = ""; //操作块对象
//单击弹出层编辑
function showEditeRegionDiv(obj) {
    opratorobj = $(obj);
    $("input:file[name='imgfile']").val("");
    $("input:hidden[name='MeetingRelationID']").val("0");
    $("input:hidden[name='RegionID']").val($(obj).attr("opratorRegion")); //操作块ID opratorRegion
    $("input:radio[name='RelationType'][value='" + $(obj).attr("relationtype") + "']").attr("checked", "checked");
    $("input:[name='RelationContent']").val("");
    $("input:[name='Direction']").val("");
    $(".dispalayRegion").hide();
    $("body").append('<div id="windownBG" style="height:' + $(document).height() + 'px;opacity:0.5;z-index: 9999;display:block;left:0px;top:0px;position:absolute;width:100%;background: none repeat scroll 0 0 #000000;"></div>');
    $("#type" + $(obj).attr("relationtype")).css({ 'top': ($(window).height() / 2) - 200 + $(window).scrollTop(), 'left': ($(window).width() / 2) - 410, 'display': 'block', 'z-index': '99999' });
    if ($(obj).attr("relationtype") == "1") {
        getVenueList(1); //加载会场列表
        return;
    }
    if ($(obj).attr("relationtype") == "2") {
        getActiveList(1);
        return;
    }
    if ($(obj).attr("relationtype") == "3") {
        GetRelationLink();
        return;
    }
    if ($(obj).attr("relationtype") == "4") {
        return;
    }
}

//保存区块关联并且替换模板
function SaveRelationContent(formobj) {
    if (formobj == "type1form") {
        if ($("#" + formobj + " input[name='RelationContent']").val().length <= 0 && $("#venueList input:radio[name='dataobj'][checked='checked']").length <= 0) {
            alert("请选择关联分会场");
            return false;
        }
    }
    if (formobj == "type2form") {
        if ($("#" + formobj + " input[name='RelationContent']").val().length <= 0 && $("#activeList input:radio[name='dataobj'][checked='checked']").length <= 0) {
            alert("请选择关联活动");
            return false;
        }
    }
    if (formobj == "type3form") {
        if ($("#" + formobj + " input[name='RelationContent']").val().length <= 0) {
            alert("请填写连接地址");
            return false;
        }
    }

    //异步提交表单
    $("#" + formobj).ajaxSubmit(function (data) {
        data = eval("(" + data + ")");
        if (data.status == 0) {
            alert(data.message);
            $(".dispalayRegion").hide();
            $('#windownBG').remove();
            return;
        }
        var venueDoMainUrl = $("#SourceFrom").val() == 1 ? $("#ShangpinUrl").val() : $("#AoLaiUrl").val();
        opratorobj.attr("relationtype", data.message.RelationType);
        opratorobj.attr("meetingrelationid", data.message.MeetingRelationID);
        if (data.message.RelationType == 1) {
            opratorobj.empty();
            opratorobj.removeClass();
            opratorobj.attr('src', data.message.ImgNOJPG);
            opratorobj.parent().attr('href', data.message.RelationContent);
        }
        if (data.message.RelationType == 2) {
            opratorobj.empty();
            opratorobj.removeClass();
            opratorobj.addClass("txtBox");
            if (data.message.WebSite == 1) {
                opratorobj.attr('src', data.message.ImgNOJPG);
                opratorobj.parent().attr('href', $("#ShangpinActiveHrefUrlShow").val() + data.message.ActiveNO + '?MeetingNO=' + data.message.MettingNO + '&ClickRegionID=' + opratorobj.attr("opratorregion"));
                //opratorobj.after('<p><em>' + data.message.ActiveName + '</em><b>' + data.message.ActiveChildName + '</b><i>点击收藏</i></p>');
            } else {
                if (data.message.IsActive == 0) {
                    opratorobj.attr('src', data.message.ImgNOJPG);
                    //opratorobj.after('<p><em>' + data.message.ActiveName + '</em><b>' + data.message.ActiveChildName + '</b><i>点击收藏</i></p>');
                } else {
                    opratorobj.attr('src', data.message.ImgNOJPG);
                    //opratorobj.after('<p><em>' + data.message.ActiveName + '</em><b>' + data.message.ActiveChildName + '</b><i>点击收藏</i></p>');
                }
            }
        }
        if (data.message.RelationType == 3) {
            opratorobj.empty();
            //opratorobj.removeClass();
            if (data.message.RelationContent.indexOf('#') >= 0) {
                //opratorobj.addClass("miniBox");
                opratorobj.parent().attr('name', data.message.RelationContent.substr(1));
                opratorobj.attr('src', data.message.ImgNOJPG);
                //opratorobj.after('<a name="' + data.message.RelationContent.substr(1) + '"></a><img src="' + data.message.ImgNOJPG + '"/>');
            } else {
                opratorobj.parent().attr('href', data.message.RelationContent);
                opratorobj.attr('src', data.message.ImgNOJPG);
                //opratorobj.after('<a href="' + data.message.RelationContent + '?MeetingNO=' + data.message.MettingNO + '&ClickRegionID=' + opratorobj.attr("opratorregion") + '"><img src="' + data.message.ImgNOJPG + '"/></a>');
            }
        }
        if (data.message.RelationType == 4) {

        }
        $('#windownBG').remove();
        $(".dispalayRegion").hide();
        //重新绑定事件
        $("#loadTemplate a").click(function (event) {
            event.preventDefault();
        });
    });
}

//发布会场
function publishVenue() {
    var mNo = $("#MeetingNO").val();
    var isPreview = $("#isPreView").val();
    var isMobile = $("#isMobile").val();
    var templateNo = "";
    if (isPreview == 0 && isMobile == 0) {
        templateNo = $("#yureyidong").val();
    }
    if (isPreview == 1 && isMobile == 0) {
        templateNo = $("#jinxingyidong").val();
    }
    if (isPreview == 0 && isMobile == 1) {
        templateNo = $("#yureweb").val();
    }
    if (isPreview == 1 && isMobile == 1) {
        templateNo = $("#jinxingweb").val();
    }
    var list = $("[opratorRegion]");
    $.ajaxSettings.async = false;
    $.get("/Shangpin/Venue/GetRegionList", { 'meetingNo': mNo, 'templateNo': templateNo }, function (data) {
        if (data != null) {
            for (var i = 0; i < data.length; i++) {
                for (var j = 0; j < list.length; j++) {
                    if ($(list[j]).attr("opratorRegion") == data[i].RegionID) {
                        $(list[j]).attr('meetingrelationid', data[i].MeetingRelationID);
                        $(list[j]).attr('src', data[i].ImgNO);
                        $(list[j]).attr('relationtype', data[i].RelationType);
                        $(list[j]).parent().attr('href', data[i].RelationContent);
                    }
                }
            }
        }
    });
    var htmldata = $("#loadTemplate").html();
    var ishtml = $("#isHTML").val();
    if (ishtml == 1) {
        if (window.confirm("您确定要在HTML编辑页面发布会场吗？")) {
            htmldata = $("#txtHtml").val();
        }
        else {
            return;
        }
    }
    if ($("#loadTemplate").text().length <= 0) {
        alert("会场内容不能为空");
        return;
    }
    var meetingplace = $("input[name='MeetingPlace']:checked").val();
    if (typeof (meetingplace) == "undefined") {
        alert("请选择要发布的会场！");
        return;
    }
    //    $.get("/Shangpin/Venue/CreateVenueHtml", { 'isPreView': $("#isPreView").val(), 'isMobile': $("#isMobile").val(), 'meetingId': $("#MeetingID").val() }, function (htmldata) {
    //        alert(htmldata);    
    $.post("/Shangpin/Venue/PublishVenue", { 'venueNO': $("#MeetingNO").val(), 'MeetingID': $("#MeetingID").val(), 'isPreView': $("#isPreView").val(), 'isMobile': $("#isMobile").val(), 'venuehtml': htmldata }, function (data) {
        if (data == "1") {
            alert("发布成功");
        } else {
            alert("发布失败" + data);
        }
    });
    //    });

}

//新增保存Html
function SaveVenue() {
    var htmldata = $("#txtHtml").val();
    if ($("#txtHtml").val().length <= 0) {
        alert("会场内容不能为空");
        return;
    }
    var meetingplace = $("input[name='MeetingPlace']:checked").val();
    if (typeof (meetingplace) == "undefined") {
        alert("请选择要发布的会场！");
        return;
    }
    $.post("/Shangpin/Venue/SaveVenue", { 'MeetingId': $("#MeetingID").val(), 'isPreView': $("#isPreView").val(), 'isMobile': $("#isMobile").val(), 'venuehtml': htmldata }, function (data) {
        if (data == "1") {
            alert("保存成功");
        } else {
            alert("保存失败" + data);
        }
    });
}
function loadImg() {

    $("img[lazy]").each(function () {
        $(this).attr("src", $(this).attr("lazy")).removeAttr("lazy");
    });
}

function initImg() {
    var imglist = $("#loadTemplate img"); //获取ID为loadTemplate里面的所有img
    var mNo = $("#MeetingNO").val();
    var isPreview = $("#isPreView").val();
    var isMobile = $("#isMobile").val();
    var templateNo = "";
    if (isPreview == 0 && isMobile == 0) {
        templateNo = $("#yureyidong").val();
    }
    if (isPreview == 1 && isMobile == 0) {
        templateNo = $("#jinxingyidong").val();
    }
    if (isPreview == 0 && isMobile == 1) {
        templateNo = $("#yureweb").val();
    }
    if (isPreview == 1 && isMobile == 1) {
        templateNo = $("#jinxingweb").val();
    }
    if (templateNo == "20140728183816") {
        for (var i = 0; i < imglist.length; i++) {
            var t = $(imglist[i]).attr("relationtype");
            //alert(typeof (t));
            if (t == null || t == "undefined") {
                t = "3";
                $(imglist[i]).attr({ relationtype: t, opratorregion: i, meetingrelationid: 0 });
            }
            $(imglist[i]).attr("meetingrelationid", 0);
        }
    }
    else {
        for (var i = 0; i < imglist.length; i++) {
            var t = $(imglist[i]).attr("relationtype");
            //alert(typeof (t));
            if (t == null || t == "undefined") {
                t = "3";
                $(imglist[i]).attr({ relationtype: t, opratorregion: i + 1 });
            }
            $(imglist[i]).attr("meetingrelationid", 0);
        }
    }
    $.get("/Shangpin/Venue/GetRegionList", { 'meetingNo': mNo, 'templateNo': templateNo }, function (data) {
        if (data != null) {
            for (var i = 0; i < data.length; i++) {
                $("[opratorRegion]").each(function () {
                    if ($(this).attr('opratorRegion') == data[i].RegionID) {
                        $(this).attr('meetingrelationid', data[i].MeetingRelationID);
                        $(this).attr('src', data[i].ImgNO);
                        $(this).attr('relationtype', data[i].RelationType);
                        $(this).parent().attr('href', data[i].RelationContent);
                    }
                });
            }
        }
    });
}
function getRadio(obj) {
    var tourl = $(obj).attr('toUrl'); //$(obj).next().first().html();
    $("[opratorRegion]").each(function () {
        if ($(this).attr('opratorRegion') == $("input:hidden[name='RegionID']").val()) {
            $(this).parent().attr('href', tourl);
        }
    });
}
function toSubject(obj) {
    var subjectNo = $(obj).attr('toUrl');
    var url = "";
    $("[opratorRegion]").each(function () {
        if ($(this).attr('opratorRegion') == $("input:hidden[name='RegionID']").val()) {
            if ($(obj).attr('webSource') == "奥莱") {
                url = $("#AoLaiActiveHrefUrlShow").val() + subjectNo;
            } else {
                url = $("#ShangpinActiveHrefUrlShow").val() + subjectNo;
            }
            $(this).parent().attr('href', url);
        }
    });
}

$(document).ready(function () {
    loadImg();
    initImg();
    $.tpl('template', [
                '<tr><td><input name="{isAdd:s}" type="radio" value="{MeetingID:s}" {IsSelect:s} {Isable:s}  onclick="getRadio(this)" toUrl="{MeetingDomain:s}"/></td><td>{MeetingDomain:s}</td><td>{MeetingName:s}</td><td>{Status:s}</td><td>{ShowTime:s}</td></tr>'
            ]);
    $.tpl('activetemplate', [
                '<tr><td><input name="{isAdd:s}" type="radio" value="{ActiveID:s}" {IsSelect:s} {Isable:s}  onclick="toSubject(this)" toUrl="{ActiveNO:s}" webSource="{WebSource:s}"/></td><td>{ActiveName:s}</td><td>{ActiveType:s}</td><td>{WebSource:s}</td><td>{ActiveNO:s}</td><td>{Status:s}</td><td>{ShowTime:s}</td></tr>'
            ]);
    $("[relationtype]").bind("click", function () {
        showEditeRegionDiv(this);
    });
    $("#loadTemplate a").click(function (event) {
        event.preventDefault();
    });
    $("input:radio[name='RelationType']").click(function () {

        $(".dispalayRegion").hide();
        $("#type" + this.value).css({ 'top': ($(window).height() / 2) - 200 + $(window).scrollTop(), 'left': ($(window).width() / 2) - 410, 'display': 'block', 'z-index': '99999' });
        if (this.value == "1") {
            $("input:radio[name='RelationType'][value=1]").attr("checked", "checked");
            getVenueList(1);
            return;
        }
        if (this.value == "2") {
            $("input:radio[name='RelationType'][value=2]").attr("checked", "checked");
            getActiveList(1);
            return;
        }
        if (this.value == "3") {
            $("input:radio[name='RelationType'][value=3]").attr("checked", "checked");
            GetRelationLink();
            return;
        }
        if (this.value == "4") {
            $("input:radio[name='RelationType'][value=4]").attr("checked", "checked");
            return;
        }
    });
    //根据下拉列表变化加载会场
    $("#isPreView").change(function () {
        if ($(this).val() == "0") {
            if ($("#isMobile").val() == "0") {//预热移动
                window.location = "/Shangpin/Venue/VenueTemplateEdite?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#yureyidong").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            } else {//预热web
                window.location = "/Shangpin/Venue/VenueTemplateEdite?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#yureweb").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            }
        } else {
            if ($("#isMobile").val() == "0") {//进行中移动
                window.location = "/Shangpin/Venue/VenueTemplateEdite?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#jinxingyidong").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            } else {//进行中web
                window.location = "/Shangpin/Venue/VenueTemplateEdite?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#jinxingweb").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            }
        }

    });
    $("#isMobile").change(function () {
        if ($(this).val() == "0") {
            if ($("#isPreView").val() == "0") {//移动预热
                window.location = "/Shangpin/Venue/VenueTemplateEdite?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#yureyidong").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            } else {//移动进行中
                templateNO = $("#jinxingyidong").val();
                window.location = "/Shangpin/Venue/VenueTemplateEdite?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#jinxingyidong").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            }
        } else {
            if ($("#isPreView").val() == "0") {//web预热
                window.location = "/Shangpin/Venue/VenueTemplateEdite?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#yureweb").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            } else {//web进行中
                window.location = "/Shangpin/Venue/VenueTemplateEdite?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#jinxingweb").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            }
        }
    });
});
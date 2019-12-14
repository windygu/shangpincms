
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
                $.tpl('template', {
                    'MeetingID': item.MeetingID, 'MeetingDomain': item.MeetingDomain,
                    'IsSelect': item.MeetingID > 0 ? '' : 'checked=checked',
                    'MeetingName': item.MeetingName, 'Status': item.Status == 1 ? '开启' : '关闭',
                    'isAdd': (item.MeetingID == -1) ? 'disobj' : 'dataobj',
                    'ShowTime': item.WebPreViewCode + "至" + item.WebStartCode, 'Isable': item.MeetingID == -1 ? 'disabled' : ''
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
                    'ShowTime': item.ActiveStartTime + "至" + item.ActiveEndTime, 'Isable': item.ActiveID == -1 ? 'disabled' : ''
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
            return;
        }
    }
    if (formobj == "type2form") {
        if ($("#" + formobj + " input[name='RelationContent']").val().length <= 0 && $("#activeList input:radio[name='dataobj'][checked='checked']").length <= 0) {
            alert("请选择关联活动");
            return;
        }
    }
    if (formobj == "type3form") {
        if ($("#" + formobj + " input[name='RelationContent']").val().length <= 0) {
            alert("请填写连接地址");
            return;
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
        opratorobj.attr("relationtype", data.message.RelationType);
        if (data.message.RelationType == 1) {
            opratorobj.empty();
            opratorobj.removeClass();
            opratorobj.append('<a href="' + $("#VenueHrefUrlShow").val() + data.message.RelationContent + '?MeetingNO=' + data.message.MettingNO + '&ClickRegionID=' + opratorobj.attr("opratorregion") + '"><img src="' + data.message.ImgNOJPG + '"/></a>');
        }
        if (data.message.RelationType == 2) {
            opratorobj.empty();
            opratorobj.removeClass();
            opratorobj.addClass("txtBox");
            if (data.message.WebSite == 1) {
                opratorobj.append('<a href="' + $("#ShangpinActiveHrefUrlShow").val() + data.message.ActiveNO + '?MeetingNO=' + data.message.MettingNO + '&ClickRegionID=' + opratorobj.attr("opratorregion") + '"><img src="' + data.message.ImgNOJPG + '"/></a>');
            } else {
                if (data.message.IsActive == 0) {
                    opratorobj.append('<a href="' + $("#AoLaiSpecialHrefUrlShow").val() + data.message.ActiveNO + '?MeetingNO=' + data.message.MettingNO + '&ClickRegionID=' + opratorobj.attr("opratorregion") + '"><img src="' + data.message.ImgNOJPG + '"/></a>');
                } else {
                    opratorobj.append('<a href="' + $("#AoLaiActiveHrefUrlShow").val() + data.message.ActiveNO + '/0?MeetingNO=' + data.message.MettingNO + '&ClickRegionID=' + opratorobj.attr("opratorregion") + '"><img src="' + data.message.ImgNOJPG + '"/></a>');
                }
            }
        }
        if (data.message.RelationType == 3) {
            opratorobj.empty();
            //opratorobj.removeClass();
            if (data.message.RelationContent.indexOf('#') >= 0) {
                //opratorobj.addClass("miniBox");
                opratorobj.append('<a name="' + data.message.RelationContent.substr(1) + '"></a><img src="' + data.message.ImgNOJPG + '"/>');
            } else {
                opratorobj.append('<a href="' + data.message.RelationContent + '?MeetingNO=' + data.message.MettingNO + '&ClickRegionID=' + opratorobj.attr("opratorregion") + '"><img src="' + data.message.ImgNOJPG + '"/></a>');
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
    if ($("#loadTemplate").text().length <= 0) {
        alert("会场内容不能为空");
        return;
    }
    var meetingplace = $("input[name='MeetingPlace']:checked").val();
    if (typeof (meetingplace) == "undefined") {
        alert("请选择要发布的会场！");
        return;
    }
    $.post("/Shangpin/Venue/CreateHTMLByTemplate", { 'venueNO': $("#MeetingNO").val(), 'templateNO': $("input[name='TemplateNO']").val() }, function (htmldata) {
        $.post("/Shangpin/Venue/PublishVenue", { 'MeetingID': $("#MeetingID").val(), 'isPreView': $("#isPreView").val(), 'isMobile': $("#isMobile").val(), 'venuehtml': htmldata }, function (data) {
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
    $.tpl('template', [
                '<tr><td><input name="{isAdd:s}" type="radio" value="{MeetingID:s}" {IsSelect:s} {Isable:s}/></td><td>{MeetingDomain:s}</td><td>{MeetingName:s}</td><td>{Status:s}</td><td>{ShowTime:s}</td></tr>'
            ]);
    $.tpl('activetemplate', [
                '<tr><td><input name="{isAdd:s}" type="radio" value="{ActiveID:s}" {IsSelect:s} {Isable:s}/></td><td>{ActiveName:s}</td><td>{ActiveType:s}</td><td>{WebSource:s}</td><td>{ActiveNO:s}</td><td>{Status:s}</td><td>{ShowTime:s}</td></tr>'
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
                window.location = "/Shangpin/Venue/VenueTemplateEditeByID?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#yureyidong").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            } else {//预热web
                window.location = "/Shangpin/Venue/VenueTemplateEditeByID?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#yureweb").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            }
        } else {
            if ($("#isMobile").val() == "0") {//进行中移动
                window.location = "/Shangpin/Venue/VenueTemplateEditeByID?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#jinxingyidong").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            } else {//进行中web
                window.location = "/Shangpin/Venue/VenueTemplateEditeByID?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#jinxingweb").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            }
        }

    });
    $("#isMobile").change(function () {
        if ($(this).val() == "0") {
            if ($("#isPreView").val() == "0") {//移动预热
                window.location = "/Shangpin/Venue/VenueTemplateEditeByID?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#yureyidong").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            } else {//移动进行中
                templateNO = $("#jinxingyidong").val();
                window.location = "/Shangpin/Venue/VenueTemplateEditeByID?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#jinxingyidong").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            }
        } else {
            if ($("#isPreView").val() == "0") {//web预热
                window.location = "/Shangpin/Venue/VenueTemplateEditeByID?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#yureweb").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            } else {//web进行中
                window.location = "/Shangpin/Venue/VenueTemplateEditeByID?venueID=" + $("#MeetingID").val() + "&currentTemplateNO=" + $("#jinxingweb").val() + "&isPreView=" + $("#isPreView").val() + "&isMobile=" + $("#isMobile").val();
            }
        }
    });
});
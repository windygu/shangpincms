
//获取关联内容
function GetRelationContent() {
    $.ajax({
        type: "POST",
        url: "/ShangPin/brand/GetRelationInfo",
        data: "templateNO=" + $("input[name='TemplateNO']").val() + "&flagReigionID=" + $("input:hidden[name='FlagReigionID']").val() + "&brandNO=" + $("input:hidden[name='BrandNO']").val() + "&regionID=" + $("input[name='RegionID']").val(),
        dataType: "json",
        success: function (data) {
            $("#yulan").attr("src", "/ReadPic/GetPic.ashx?width=100&height=100&pictureFileNo=" + data.ImgNO + "&type=2");
            $("input:hidden[name='ImgNO']").val(data.ImgNO);
            $("input:hidden[name='FlagReigionID']").val(data.FlagReigionID);
            $("input:hidden[name='RelationType']").val(data.RelationType);
            if ($("input[name='selectedType'][checked='checked']").val() == data.RelationType) {
                if (data.RelationType == 3) {
                    //获取手动导航数据
                    $.get("/shangpin/brand/GetNavTree?brandNO=" + $("#BrandNO").val() + "&selectValue=" + data.RelationContent, function (data) {
                        $("#navID").html(data);
                    });
                } else {
                    $("#RelationContent").val(data.RelationContent);
                }
            } else {
                if ($("input[name='selectedType'][checked='checked']").val() == "3") {
                    //获取手动导航数据
                    $.get("/shangpin/brand/GetNavTree?brandNO=" + $("#BrandNO").val() + "&selectValue=0", function (data) {
                        $("#navID").html(data);
                    });
                }
                $("#RelationContent").val("");
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
    $("input:[name='RelationContent']").val("");
    $("input:radio[name='selectedType']").removeAttr("checked");
    $("#navID").hide();
    $("#checkactive").hide();
    $("#tipActiveName").html("");
    if ($(obj).attr("relationtype") == "1") {//活动
        $("input:radio[name='selectedType'][value='1']").attr("checked", "checked");
        $("#RelationContent").show();
        $("#checkactive").show();
        $("#navID").hide();
        GetRelationContent();
    }
    if ($(obj).attr("relationtype") == "2") {//单品
        $("input:radio[name='selectedType'][value='2']").attr("checked", "checked");
        $("#typetext").text("单个商品编号:");
        $("#RelationContent").show();
        $("#checkactive").hide();
        $("#navID").hide();
        GetRelationContent();
    }
    if ($(obj).attr("relationtype") == "3") {//手动导航
        $("input:radio[name='selectedType'][value='3']").attr("checked", "checked");
        $("#typetext").text("选择内容:");
        $("#RelationContent").hide();
        $("#checkactive").hide();
        $("#navID").show();
        GetRelationContent();
    }
    if ($(obj).attr("relationtype") == "4") {//专用链接
        $("input:radio[name='selectedType'][value='4']").attr("checked", "checked");
        $("#typetext").text("专用链接:");
        $("#RelationContent").show();
        $("#checkactive").hide();
        $("#navID").hide();
        GetRelationContent();
    }
    $("body").append('<div id="windownBG" style="height:' + $(document).height() + 'px;opacity:0.5;z-index: 9999;display:block;left:0px;top:0px;position:absolute;width:100%;background: none repeat scroll 0 0 #000000;"></div>');
    $("#regionEdite").css({ 'top': ($(window).height() / 2) - 200 + $(window).scrollTop(), 'left': ($(window).width() / 2) - 410, 'display': 'block', 'z-index': '99999' });
}

//保存区块关联并且替换模板
function SaveRelationContent() {
    if ($("input[name='selectedType'][checked='checked']").val() == "1") {
        if ($("#RelationContent").val().length <= 0) {
            alert("请输入活动编号");
            return;
        }
        //检查活动编号
        $.get("/shangpin/brand/CheckActiveNO?activeNO=" + $("#RelationContent").val(), function (data) {
            if (data == "0") {
                alert("不存在该活动编号");
            } else {
                ajaxSaveFormContent();
            }
        });
    }
    if ($("input[name='selectedType'][checked='checked']").val() == "2") {
        if ($("#RelationContent").val().length <= 0) {
            alert("请输入商品编号");
            return;
        }
        ajaxSaveFormContent();
    }
    if ($("input[name='selectedType'][checked='checked']").val() == "3") {
        if ($("#navID").val()== "0") {
            alert("请选择导航内容");
            return;
        }
        ajaxSaveFormContent();
    }
    if ($("input[name='selectedType'][checked='checked']").val() == "4") {
//        if ($("#RelationContent").val().length <= 0) {
//            alert("请输入链接地址");
//            return;
//        }
        ajaxSaveFormContent();
    }

}
//异步提交表单
function ajaxSaveFormContent() {
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
            opratorobj.append('<a href="javascript:;"><img src="/ReadPic/GetPic.ashx?width=0&height=0&pictureFileNo=' + data.message.ImgNO + '&type=2""/></a>');
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



$(document).ready(function () {
    loadImg();
    $("[relationtype]").bind("click", function () {
        showEditeRegionDiv(this);
    });
    $("#loadTemplate a").click(function (event) {
        event.preventDefault();
    });
    $("input:radio[name='selectedType']").click(function () {
        //$("#regionEdite").css({ 'top': ($(window).height() / 2) - 200 + $(window).scrollTop(), 'left': ($(window).width() / 2) - 410, 'display': 'block', 'z-index': '99999' });
        $("input:radio[name='selectedType']").removeAttr("checked");
        if (this.value == "1") {//活动
            $("input:radio[name='selectedType'][value='1']").attr("checked", "checked");
            $("#typetext").text("活动编号:");
            $("#RelationContent").show();
            $("#checkactive").show();
            $("#navID").hide();
            GetRelationContent();
            return;
        }
        if (this.value == "2") {//单品
            $("input:radio[name='selectedType'][value='2']").attr("checked", "checked");
            $("#typetext").text("单个商品编号:");
            $("#RelationContent").show();
            $("#checkactive").hide();
            $("#navID").hide();
            GetRelationContent();
            return;
        }
        if (this.value == "3") {//手动导航
            $("input:radio[name='selectedType'][value='3']").attr("checked", "checked");
            $("#typetext").text("选择内容:");
            $("#RelationContent").hide();
            $("#checkactive").hide();
            $("#navID").show();
            GetRelationContent();
        }
        if (this.value == "4") {//专用链接
            $("input:radio[name='selectedType'][value='4']").attr("checked", "checked");
            $("#typetext").text("专用链接:");
            $("#RelationContent").show();
            $("#checkactive").hide();
            $("#navID").hide();
            GetRelationContent();
        }

    });


});

//检查活动编号
function checkActiveNo() {
    if ($("#RelationContent").val().length <= 0) {
        alert("请输入活动编号");
        return;
    }
    $.get("/shangpin/brand/CheckActiveNO?activeNO=" + $("#RelationContent").val(), function (data) {
        if (data == "0") {
            $("#tipActiveName").text("不存在该活动编号");
        } else {
            $("#tipActiveName").text(data);
        }
    });
}
//发布会场
function publishVenue() {
    if ($("#loadTemplate").text().length <= 0) {
        alert("会场内容不能为空");
        return;
    }
    $.post("/Shangpin/brand/CreateHTMLByTemplate?brandNO=" + $("#BrandNO").val() + "&brandName=" + $("#BrandName").val(), { 'brandNO': $("#BrandNO").val(), 'templateNO': $("#TemplateNO").val() }, function (htmldata) {
        $.post("/Shangpin/brand/PublishFlagShip", { 'flagID': $("#FlagShipSotreID").val(), 'htmlCode': htmldata }, function (data) {
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
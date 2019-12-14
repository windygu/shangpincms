
$(document).ready(function () {
    $("#tabs li").bind("click", function () {
        var index = $(this).index();
        var divs = $("#tabs-body > div");
        $(this).parent().children("li").attr("class", "tab-nav");//将所有选项置为未选中
        $(this).attr("class", "tab-nav-action"); //设置当前选中项为选中样式
        divs.hide();//隐藏所有选中项内容
        divs.eq(index).show(); //显示选中项对应内容
    });

    $(function () {
        $("#FormAdd").submit(function () {

            var linkname = $("#LinkName").val();
            if ($.trim(linkname) == "") {
                alert("名称不能为空！");
                return false;
            }
            else {
                var linknamelength = linkname.length;
                if (linknamelength > 16) {
                    alert("名称限制在16个中文字符以内");
                    return false;
                }
            }

            var addr = $("#LinkAddress").val();
            if ($.trim(addr) == "") {
                alert("链接地址不能为空！");
                return false;
            }
            else {
                var suffix = addr.substr(0, 7).toUpperCase();
                if (suffix != "HTTP://") {
                    alert("链接必须以http://开头");
                    return false;
                }
            }
            if (addr.length > 200) {
                alert("链接地址太长了！");
                return false;
            }

            var beginTime = $("#BeginTime").val();
            if (beginTime == "") {
                alert("开始时间不能为空！");
                return false;
            }

            var endTime = $("#EndTime").val();
            if (endTime == "") {
                alert("结束时间不能为空！");
                return false;
            }

            if (beginTime > endTime) {
                alert("起始时间不能大于结束时间！");
                return false;
            }

            $.ajax({
                url: '/NewChannel/AddsWfsRecommLink2?channelno=' + $("#hidChannelNo").val(),
                type: "POST",
                data: $(this).serialize(),
                beforeSend: function (a, b, c) {
                },
                complete: function () {
                },
                success: function (result) {
                    if (result.Successed) {
                        alert("保存成功！");
                        window.location = window.location.href;
                    }
                    else {

                    }
                },
                error: function (result) {

                }
            });
            return false;
        })
    });
})


function Add() {
    $("#FormAdd").css("display", "block");
}



var save = function () {

    var file = $("#PicFile").val();
    if (file == "") {
        alert("上传的图片不能为空！");
        return;
    }
    else {
        var suffix = file.substr(file.lastIndexOf(".")).toUpperCase();;
        if (suffix != ".JPG") {
            alert("图片限于jpg格式");
            return;
        }
    }

    var imgaddr = $("#lianjie").val();
    if ($.trim(imgaddr) == "") {
        alert("链接地址不能为空！");
        return;
    }
    else {
        var suffix = imgaddr.substr(0, 7).toUpperCase();
        if (suffix != "HTTP://") {
            alert("链接必须以http://开头");
            return;
        }
    }

    if (imgaddr.length > 100) {
        alert("链接地址太长了！");
        return false;
    }

    $("#UploadForm").ajaxSubmit(function (data) {
        if (data == "请上传180*350的图片！！" || data == "请上传小于50KB的图片！！") {
            alert(data);
            return false;
        }

        $("#viewImg").attr("src", "/ReadPic/GetPic.ashx?width=210&height=280&pictureFileNo=" + data + "&type=2");
        //$("#viewImg").show();
        $("#imglink").attr("href", $("#lianjie").val());
        alert("保存成功，点击预览可查看图片！");
    });
}

//预览
function yulans() {
    if ($("#viewImg").attr("src") == "") {
        alert("请先保存图片！");
        return false;
    }
    $("#viewImg").show();
}



//上移
//function GotoTop(obj) {
//    //$(obj).parent().parent().parent().find("th").first().index()
//    if ($(obj).parent().parent().index() > 1) {
//        $(obj).parent().parent().prev().before($(obj).parent().parent());
//        $("#SaveTable").css("display", "inline");
//    }
//}
////下移
//function GotoBottom(obj) {
//    $(obj).parent().parent().next().after($(obj).parent().parent());
//    $("#SaveTable").css("display", "inline");
//    //$(obj).parent().parent().parent().find("tr").last();
//}

//上移
function GotoTop(obj) {

    if ($(obj).parent().parent().index() > 1) {
        var curObj = $(obj).parent().parent();
        var fNo = $(curObj).find("td").first().html().split(',')[0];
        var fSortId = $(curObj).find("td").first().html().split(',')[1];
        var sNo = $(curObj).prev().find("td").first().html().split(',')[0];
        var sSortId = $(curObj).prev().find("td").first().html().split(',')[1];
        if (SaveMoveItem(fNo, fSortId, sNo, sSortId)) {
            $(curObj).prev().before(curObj);
        }
    }
}
//下移
function GotoBottom(obj) {

    if ($(obj).parent().parent().index() < ($(obj).parent().parent().parent().find("tr").size() - 1)) {
        var curObj = $(obj).parent().parent();
        var fNo = $(curObj).find("td").first().html().split(',')[0];
        var fSortId = $(curObj).find("td").first().html().split(',')[1];
        var sNo = $(curObj).next().find("td").first().html().split(',')[0];
        var sSortId = $(curObj).next().find("td").first().html().split(',')[1];

        if (SaveMoveItem(fNo, fSortId, sNo, sSortId)) {
            $(curObj).next().after(curObj);
        }
    }

}

//新的保存移动元素 直接写入数据库
function SaveMoveItem(firstNo, firstSortId, secondNo, secondSortId) {
    var resultd = false;
    $.ajax({
        url: '/NewChannel/SaveMoveItem',
        type: "POST",
        async: false,
        dataType: "text",
        data: { firstNo: firstNo, firstSortId: firstSortId, secondNo: secondNo, secondSortId: secondSortId },
        success: function (result) {
            if (result = "0") {
                resultd = true;
            }
        },
        error: function (result) {
            LG.tip('发现系统错误 <BR>错误码：');
        }
    });
    return resultd;
}


//删除
function DelBottom(obj) {
    if (confirm("您确定要删除吗？")) {
        var id = "";
        id = $(obj).attr("nid");
        $.ajax({
            url: '/NewChannel/DeleteLink',
            type: "POST",
            data: { ID: id },
            success: function (result) {
                if (result.Successed) {
                    alert("删除成功！");
                    window.location = window.location.href;
                }
                else {
                    alert("删除失败！");
                    LG.showError('错误:' + result.Message);
                }
            },
            error: function (result) {
                LG.tip('发现系统错误 <BR>错误码：' + result.status);
            }
        });
        return false;
    }
}

//修改状态
function UpdateState(obj) {
    var id = "";
    var status = "";
    id = $(obj).attr("nid");
    status = $(obj).attr("nstatus") == "0" ? "1" : "0";
    var xsname = status == 1 ? "显示" : "隐藏";
    $.ajax({
        url: '/NewChannel/UpdateNaviState',
        type: "POST",
        data: { id: id, status: status },
        success: function (result) {
            if (result.Successed) {
                $(obj).html(xsname);
                $(obj).attr("nstatus", status)
            }
            else {
                LG.showError('错误:' + result.Message);
            }
        },
        error: function (result) {
            LG.tip('发现系统错误 <BR>错误码：' + result.status);
        }
    });
    return false;
}

////保存
//function savetable() {
//    var list = "";
//    var i = 100;
//    $("#table_main tr").each(function () {
//        var text = $(this).children("td:first").text();
//        list += i + "-";
//        list += text + ",";
//        i--;
//    });

//    $.ajax({
//        url: '/NewChannel/Sorting',
//        type: "POST",
//        data: { Sort: list },
//        success: function (result) {
//            if (result.Successed) {
//                alert("保存成功！");
//                window.location = window.location.href;
//            }
//            else {
//                //LG.showError('错误:' + result.Message);
//            }
//        },
//        error: function (result) {
//            //LG.tip('发现系统错误 <BR>错误码：' + result.status);
//        }
//    });
//    return false;
//}


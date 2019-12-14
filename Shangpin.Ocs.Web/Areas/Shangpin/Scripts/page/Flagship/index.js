
$(function () {
    getChannelStates();
    sortFloor();
    $(".button").attr("onselectstart", "return false;");
})
//修改楼层名称
function ChangeModuleName(obj, ModuleId) {
    var $this = $(obj);
    if (!$this.is("[save=save]")) {
        $this.prev("input").remove();
        $this.text("保存").attr("save", "save");
        var span = $this.prev("h1").hide();
        $this.before("<input style='width:200px;color: #212121;font-size: 16px;line-height: 24px;' type='input' value='" + span.text() + "' maxlength='10'>");
        $this.after('<a class="button blue_button cancel" onclick="cancelEditModule(this)" style="font-size:12px;color:white;" href="javascript:void(0);">取消</a>');

    } else {
        var input = $this.prev("input");
        input.val($.trim(input.val()));//去空格 
        if ($this.is("[save=save]") && input.val().length > 0 && input.val().length <= 10) {
            $.ajax({
                url: "/shangpin/Flagship/alterFloorName",
                data: { ModuleId: ModuleId, ModuleName: input.val() },
                type: "get",
                success: function (result) {
                    if (result.status == 1) {
                        $this.text($this.attr("oldname"));
                        $this.next("a.cancel").remove();
                        input.prev("h1").text(input.val()).show();
                        $this.removeAttr("save");
                        input.remove();
                    } else {
                        alert("操作失败");
                    }
                },
                "error": function () {
                    alert("提交失败,网络故障或后台出现错误");
                }
            })
        }
    }
}
//取消按钮
function cancelEditModule(obj) {
    var prevA = $(obj).prev("a");
    prevA.text(prevA.attr("oldname"));
    prevA.prev("input").remove();
    prevA.prev("h1").show();
    prevA.removeAttr("save");
    $(obj).remove();
}
//楼层隐藏关闭开关
function alterFloorState(obj, ModuleId) {
    if (confirm("确定切换楼层状态吗?状态更改将立即生效")) {
        $.ajax({
            url: "/shangpin/Flagship/AlterFloorState?rand=" + Math.random(),
            data: { ModuleId: ModuleId, value: ($(obj).text() == "隐藏" ? "0" : "1") },
            success: function (result) {
                if (result.status == 1) {
                    var $obj = $(obj);
                    $obj.text($obj.text() == "隐藏" ? "显示" : "隐藏");
                    sortFloor();
                }
                else {
                    alert('操作失败请稍后再试');
                }
            },
            "error": function () {
                alert("提交失败,网络故障或后台出现错误");
            }
        })
    }
}
//修改楼层排序
function alterFloorSort(isup, obj, ModuleId) {
    var parent = $(obj).parentsUntil("[floorsort]").last().parent();
    var sort = parent.attr("floorsort");
    if (isup) {//上移
        sort--;
    } else {
        sort++;

    }
    if (sort > 3 || sort < 1) { return }
    if (!$(obj).is("[doing]")) {//&& confirm("确定更新排序吗?排序更改将立即生效")) {
        $(obj).attr("doing", "doing");
        $.ajax({
            url: "/shangpin/Flagship/AlterFloorSort?rand=" + Math.random(),
            data: { ModuleId: ModuleId, Sort: sort },
            success: function (result) {
                if (result.status == 1) {
                    $("[floorsort='" + sort + "']").prev().find("a:eq(0)").attr("name", "l" + parent.attr("floorsort"));
                    $("[floorsort='" + sort + "']").attr("floorsort", parent.attr("floorsort"));
                    parent.attr("floorsort", sort);
                    parent.prev().find(" a:eq(0)").attr("name", "l" + sort);
                    sortFloor();
                    location.href = "#" + parent.prev("tr").find(" a:eq(0)").attr("name");
                    parent.mouseout().mouseover();

                }
                else {
                    alert('操作失败请稍后再试');
                }
                $(obj).removeAttr("doing");
            },
            "error": function () {
                alert("提交失败,网络故障或后台出现错误");
                $(obj).removeAttr("doing");
            }
        })
    }
}
var focusA = null;
$(function () {
    $("[floorsort]").live({
        mouseover: function () {
            focusA = $("a[href='" + "#" + $(this).prev("tr").find(" a:eq(0)").attr("name") + "']");
            focusA.css("background", " white");
        }, mouseout: function () {
            if (focusA != null) {
                focusA.css("background", "");
            }
        }
    })
})
////修改旗舰店状态
//function changeStates(obj, BrandNo) {
//    if (confirm("确定切换状态吗?状态更改将立即生效")) {
//        $.ajax({
//            url: "/shangpin/Flagship/ChangeFlagShopStatus?rand=" + Math.random(),
//            data: { BrandNo: BrandNo, value: ($(obj).text() == "取消发布" ? "0" : "1") },
//            success: function (result) {
//                if (result.status == 1) {
//                    $(obj).text($(obj).text() == "发布" ? "取消发布" : "发布");
//                    $("#currentStatus").text(($(obj).text() != "取消发布" ? "当前状态:已发布" : "当前状态:未发布"));
//                }
//                else {
//                    alert('操作失败请稍后再试');
//                }
//            },
//            "error": function () {
//                alert("提交失败,网络故障或后台出现错误");
//                $(obj).text($(obj).text() != "发布" ? "取消发布" : "发布");
//                $("#currentStatus").text("");
//            }
//        })
//    }
//}
function getChannelStates() {
    if ($("#BrandNo").length > 0 && $("#BrandNo").val().length > 0) {
        var BrandNo = $("#BrandNo").val();
        $.ajax({
            url: "/shangpin/Flagship/GetFlagShopStatus",
            data: { BrandNo: BrandNo },
            success: function (result) {
                if (result.status == 1 && result.data) {
                    $("#BrandName").text("品牌名:" + result.data.name);
                    // $("input[type=radio][name=BrandNo][value='" + result.data.value + "']").attr("checked", "checked");
                    $("#BrandStatus").text((result.data.value == '1' ? "取消发布" : "发布"));
                    $("#currentStatus").text((result.data.value == '1' ? "当前状态:已发布" : "当前状态:未发布"));
                }
                else {
                }
            },
            "error": function () {
                alert("提交失败,网络故障或后台出现错误");
            }
        })
    }
}
//楼层排序
function sortFloor() {
    var floors = $("[floorsort]");
    for (var i = 1; i < floors.length; i++) {
        $("[floorsort='" + i + "']").after($("[floorsort='" + (i + 1) + "']").prev().andSelf());
    }
    $("a[status]").each(function (i, n) {
        if ($(n).text() == "显示") {
            $(n).parentsUntil("tr[floorsort]").last().parent().css("border", "2px dashed red");
        } else {
            $(n).parentsUntil("tr[floorsort]").last().parent().css("border", "");
        }
    })
}
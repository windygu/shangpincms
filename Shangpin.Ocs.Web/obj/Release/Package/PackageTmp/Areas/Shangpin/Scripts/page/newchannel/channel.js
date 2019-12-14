
$(function () {
    $(".floor:last:not(.hidefloor)").after($(".hidefloor"));
    getChannelStates();
})

function ChangeModuleName(obj, ModuleId) {
    var $this = $(obj);
    if (!$this.is("[save=save]")) {
        $this.prev("input").remove();
        $this.text("保存").attr("save", "save");
        var span = $this.prev("span.left").hide();
        $this.before("<input style='width:200px;color: #212121; font-family: '微软雅黑','Microsoft YaHei', '黑体', 'Tahoma', '宋体';font-size: 16px;line-height: 24px;' type='input' value='" + span.text() + "' maxlength='10'>");
        $this.after('<a class="blue_button cancel" onclick="cancelEditModule(this)" style="font-size:12px;color:white;" href="javascript:void(0);">取消</a>');

    } else {
        var input = $this.prev("input");
        input.val($.trim(input.val()));//去空格 
        if ($this.is("[save=save]") && input.val().length > 0 && input.val().length <= 10) {
            $.ajax({
                url: "/shangpin/NewChannels/alterFloorName",
                data: { ModuleId: ModuleId, ModuleName: input.val() },
                type: "get",
                success: function (result) {
                    if (result.status == 1) {
                        $this.text($this.attr("oldname"));
                        $this.next("a.cancel").remove();
                        input.prev("span.left").text(input.val()).show();
                        $this.removeAttr("save");
                        input.remove();
                    } else {
                        alert("操作失败");
                    }
                }
            })
        }
    }
}
function cancelEditModule(obj) {
    var prevA = $(obj).prev("a");
    prevA.text(prevA.attr("oldname"));
    prevA.prev("input").remove();
    prevA.prev("span.left").show();
    prevA.removeAttr("save");
    $(obj).remove();
}

function changeStates(channelNo, value) {
    if (channelNo.length > 0) {
        $.ajax({
            url: "/shangpin/NewChannels/ChangeChannelStatus?rand=" + Math.random(),
            data: { PageNo: channelNo, value: value },
            success: function (result) {
                if (result.status == 1) { }
                else {
                    alert('操作失败请稍后再试');
                }
            }
        })
    }
}
function getChannelStates() {
    if ($("#PageNo").length > 0 && $("#PageNo").val().length > 0) {
        var channelNo = $("#PageNo").val();
        $.ajax({
            url: "/shangpin/NewChannels/GetChannelStatus?rand=" + Math.random(),
            data: { PageNo: channelNo },
            success: function (result) {
                if (result.status == 1 && result.data) {
                    $("#channelName").text(result.data.name);
                    $("input[type=radio][name=channel][value='" + result.data.value + "']").attr("checked", "checked");
                }
                else {
                }
            }
        })
    }
}
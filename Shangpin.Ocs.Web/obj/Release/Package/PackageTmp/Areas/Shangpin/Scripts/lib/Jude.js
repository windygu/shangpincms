//判断文本框是否为空
function FlagEmpty(Id) {
    var msg = $("#" + Id).val()
    if (msg == "" || msg == null || msg == undefined) {
        return false;
    }
    return true;
}
//判断字符个数 与长度匹配
function FlagCountChar(id, len) {
    var msg = $("#" + id).val()
    var nums = msg.replace(/[^\x00-\xff]/g, "00").length;
    if (nums > len) {
        return false;
    }
    return true;
}
//判断字符个数
function CountCharLen(id) {
    var msg = $("#" + id).val()
    return msg.replace(/[^\x00-\xff]/g, "00").length;

}
//判断复选框有没有都不选
function FlagCheckBoxCheck(Name) {
    var SordNos = "";
    $("input[name='" + Name + "']:checked").each(function () {
        SordNos += $(this).val() + ",";
    });
    if (SordNos == "") {
        return false;
    }
    return true;
}
//判断文本框的值是否相等
function FlagEqules(ido, idt) {
    var msgo = $("#" + ido).val()
    var msgt = $("#" + idt).val()
    if (msgo != msgt) {
        return false;
    }
    return true;
}
//判断是否隐藏
function FlagView(id) {
    if ($("#" + id).is(":hidden")) {
        return false;
    }
    return true;
}
//只能输入英文
function CheckEn(id) {
    var msg = $("#" + id).val()
    if (/[^\a-\z\A-\Z]/g.test(msg)) {
        return false;
    }
    return true;
}
//只能输入英文和数字
function CheckEnAndNum(id) {
    var msg = $("#" + id).val()
    if (/^[0-9a-zA-Z_]$/.test(msg)) {
        return false;
    }
    return true;
}

//判断文本框长度
function CheckValLength(id, min, max) {
    var msg = $("#" + id).val()
    if (msg.length > max || msg.length < min) {
        return false;
    }
    return true;
}
//只能输入数字
function CheckNo(id) {
    var msg = $("#" + id).val()
    if (!/^[0-9_]*$/.test(msg)) {
        return false;
    }
    return true;
}
//验证小数
function CheckFlagNum(id) {
    var msg = $("#" + id).val()
    if (!/^[0-9]+(.[0-9]{1})?$/.test(msg)) {
        return false;
    }
    return true;
}

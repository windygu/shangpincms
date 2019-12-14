///输入字符，限制长度，提示信息
function CheckStr(intputStr,length,msg) {

    var Zhlength = 0; // 全角
    var Enlength = 0; // 半角

    for (var i = 0; i < intputStr.length; i++) {
        if (intputStr.substring(i, i + 1).match(/[^\x00-\xff]/ig) != null)
            Zhlength += 1;
        else
            Enlength += 1;
    }
    var byte = (Zhlength * 2) + Enlength;
    if (byte > length) {
        alert(msg);
        return false;
    }
    return true;

}
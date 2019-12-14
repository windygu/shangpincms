(function (window, $, undefined) {
    $(function () {
        //拖动选择加载项

        $("#SortBrand").sortable({
            placeholder: "ui-sortable-placeholder",
        });

        $("#SortCategory").sortable({
            placeholder: "ui-sortable-placeholder",
        });
        InitPage(); //初始化页面状态

        $("#ol-index-today-tab li").mousemove(function (e) {
            var obj = $(this);
            if ($(obj).attr("class") != "curr") {
                $(obj).addClass("mouseUp");
            }
        });
        $("#ol-index-today-tab li").mouseout(function () {
            var obj = $(this);
            if ($(obj).attr("class") != "curr") {
                $(obj).removeClass();
            }
        });
        if ($("#hidIAB").val() == "0") {
            $("input[name=ckbSweepwood]").attr("checked", false);
        }

        var Request = new Object();
        Request = GetRequest();

        var urlParaIAB = Request['IsAutoBottom'];    //  判断当前页面是否属于同页面操作
        if (urlParaIAB != null && urlParaIAB != "") {

            if (urlParaIAB == "0") {
                $("input[name=ckbSweepwood]").attr("checked", false);
            }
            else { $("input[name=ckbSweepwood]").attr("checked", "checked"); }
        }
        var last = $("#hidSortRuleType").val();
        if (last != null && last != "" && last != "NULL") {
            SelectRadio(last);
        }
        var urlParaSort = Request['SortType'];    //  判断当前页面是否属于同页面操作
        if (urlParaSort != null && urlParaSort != "") {
            SelectRadio(urlParaSort);
        }
        else {
            SelectRadio($("#hidSortRuleType").val());
        }

    });
})(window, jQuery);

//页面初始化
function InitPage() {
    $("#radSortGD").hide();
    $("#radSortXJ").hide();
    $("#divSortBrand").hide();
    $("#divSortCategory").hide();
    $("#divColor").hide();
    $("#hidSortType").val("");
    $("input[name=ridGD]").attr("checked", false);
    $("input[name=ridXJ]").attr("checked", false);
    $("#test").hide();

}


//获取url参数
function GetRequest() {
    var url = location.search; //获取url中"?"符后的字串
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    return theRequest;
}
// 页面初始化根据参数值加载
function SelectRadio(data) {
    if (data == null) {
        return;
    }
    var tempId = "";
    var d = data.split('|');
    if (d.length == 2) {
        if (d[0] == "" || d[1] == "") { }
        if (d[0] == "2") {

            $("#hidSelectData").val("divSortBrand");
            $("#test").show();
            $("#sltSortType").val(d[0]);
            return;
        }
        if (d[0] == "7") {
            $("#hidSelectData").val("divSortCategory");
            $("#test").show();
            $("#sltSortType").val(d[0]);
            return;
        }
        if (d[0] == "8") {
            $("#hidSelectData").val("divColor");
            $("#test").show();
            $("#sltSortType").val(d[0]);
            var _ColorData = $("#hidColorData").val();
            if (_ColorData != null && _ColorData != "") {
                var ColorArra = _ColorData.split("|");
                var html = "";
                for (var i = 0; i < ColorArra.length; i++) {
                    if (ColorArra[i] != null && ColorArra[i] != "") {
                        var singleColor = ColorArra[i].split(",");
                        if (singleColor.length == 2) {
                            html += "<li class=\"ui-state-default\" title=" + singleColor[1] + " id=\"" + singleColor[0] + "\"  draggable=\"true\" >" + singleColor[1] + "</li>";
                        }
                    }
                }
                $("#SortColor").html(html);

                $("#SortColor").sortable({
                    placeholder: "ui-sortable-placeholder",
                });
            }
            else {
                if ($("#hidColorAll").val() != "" && $("#hidColorAll").val() != null) {
                    $("#radSortGD").hide();
                    $("#radSortXJ").hide();
                    $("#divSortBrand").hide();

                    //  $("#divColor").fadeIn(2000);
                    var _SortRuleType = $("#hidSortRuleType").val();
                    var _SortRuleTypeArra = _SortRuleType.split("|");
                    if (_SortRuleTypeArra.length == 2) {
                        $.ajax({
                            url: "/Outlet/Subject/AjaxSearchColorDetailInit",
                            type: "post",
                            dataType: "json",
                            data: "colorall=" + _SortRuleTypeArra[1],
                            success: function (data) {
                                if (data != null) {
                                    if (data.result == true) {
                                        var _color = data.message;
                                        var ColorArra = _color.split("|");
                                        var html = "";
                                        for (var i = 0; i < ColorArra.length; i++) {
                                            if (ColorArra[i] != null && ColorArra[i] != "") {
                                                var singleColor = ColorArra[i].split(",");
                                                if (singleColor.length == 2) {
                                                    html += "<li class=\"ui-state-default\" title=" + singleColor[1] + " id=\"" + singleColor[0] + "\" draggable=\"true\">" + singleColor[1] + "</li>";
                                                }
                                            }
                                        }
                                        $("#SortColor").html(html);

                                        $("#SortColor").sortable({
                                            placeholder: "ui-sortable-placeholder",
                                        });
                                    } else {
                                        alert(data.message);
                                        return false;
                                    }
                                }
                            },
                            error: function (x, e) {
                                alert(x.responseText);
                            }
                        });

                        $("#test").show();
                        $("#hidSelectData").val("divColor");
                        //  $("#divColor").fadeIn(2000);
                    }
                }
            }
        }
        if (d[0] == "9") {

            $("#sltSortType").val(d[0]);
            $("#hidSortType").val(data);
        }
        else {
            tempId = isOrderRad(data);
            if (tempId == "DG")
                tempId = "GD";
            if (tempId == "JX")
                tempId = "XJ";

            $("#radSort" + tempId).show();
            $("input[name='rid" + tempId + "'][value=" + d[1] + "]").attr("checked", 'checked');
            // $("input[type=radio][value=" + d[1] + "]").attr("checked", 'checked');
            $("#sltSortType").val(d[0]);

            $("#hidSortType").val(data);
        }
    }

}
// 单选按钮选中，填值
function radioChecked(id) {

    var sortData = "";
    sortData = $("#hidSortType").val();

    if (sortData.indexOf("|") > 0) {
        sortData = sortData.substring(0, sortData.indexOf("|"));
    }
    sortData += "|" + $("input[name='rid" + id + "']:checked").val();

    $("#hidSortType").val(sortData);

}
// 隐藏的div
function hidDiv(id) {
    $("#" + id + "").hide();
}
// 淡进
function OnDataShow() {
    var selectData = $("#hidSelectData").val();
    $("#" + selectData + "").fadeIn(1000);
}

//选择类型更改显示
function OnChangeData(val, identifie) {

    $("#hidSortType").val(val);
    $("input[name=ridGD]").attr("checked", false);
    $("input[name=ridXJ]").attr("checked", false);
    if (identifie == "1") {
        $("#radSortGD").show();
        $("#radSortXJ").hide();
        $("#divSortBrand").hide();
        $("#divSortCategory").hide();
        $("#divColor").hide();
        $("#test").hide();

    }
    if (identifie == "2") {
        $("#radSortGD").hide();
        $("#radSortXJ").show();
        $("#divSortBrand").hide();
        $("#divSortCategory").hide();
        $("#divColor").hide();
        $("#test").hide();

    }
    if (identifie == "3") {
        $("#radSortGD").hide();
        $("#radSortXJ").hide();
        $("#divSortCategory").hide();
        $("#divColor").hide();
        // $("#divSortBrand").show();

        $("#test").show();
        $("#hidSelectData").val("divSortBrand");
        // $("#divSortBrand").fadeIn(2000);

    }
    if (identifie == "4") {
        $("#radSortGD").hide();
        $("#radSortXJ").hide();
        $("#divSortBrand").hide();
        $("#divColor").hide();
        // $("#divSortCategory").show();

        $("#test").show();
        $("#hidSelectData").val("divSortCategory");

    }
    if (identifie == "5") {
        $("#radSortGD").hide();
        $("#radSortXJ").hide();
        $("#divSortBrand").hide();
        $.ajax({
            url: "/Outlet/Subject/AjaxSearchColorDetail",
            type: "post",
            dataType: "json",
            data: "colorall=" + $("#hidColorAll").val(),
            success: function (data) {
                if (data != null) {
                    if (data.result == true) {
                        var _color = data.message;
                        var ColorArra = _color.split("|");
                        var html = "";
                        for (var i = 0; i < ColorArra.length; i++) {
                            if (ColorArra[i] != null && ColorArra[i] != "") {
                                var singleColor = ColorArra[i].split(",");
                                if (singleColor.length == 2) {
                                    html += "<li class=\"ui-state-default\" title=" + singleColor[1] + " id=\"" + singleColor[0] + "\" draggable=\"true\">" + singleColor[1] + "</li>";
                                }
                            }
                        }
                        $("#SortColor").html(html);

                        $("#SortColor").sortable({
                            placeholder: "ui-sortable-placeholder",
                        });
                    } else {
                        alert(data.message);
                        return false;
                    }
                }
            },
            error: function (x, e) {
                alert(x.responseText);
            }
        });

        $("#test").show();
        $("#hidSelectData").val("divColor");
        //  $("#divColor").fadeIn(2000);

    }

    if (identifie == "-1" || identifie == "") {
        $("#radSortGD").hide();
        $("#radSortXJ").hide();
        $("#divSortBrand").hide();
        $("#divSortCategory").hide();
        $("#divColor").hide();
        $("#test").hide();
    }

}
// 排序确定
function btnSubmit() {

    var tempSortType = "";
    tempSortType = $("#hidSortType").val();

    if (tempSortType == "-1") {
        alert("请选择排序类型");
        return false;
    }

    if (tempSortType == "") {
        tempSortType = $("#hidSortRuleType").val();
    }

    var isAutoBottom = "1";
    if (!$("input[name=ckbSweepwood]").attr("checked")) {
        isAutoBottom = "0";
    }

    var SortTypeArray = tempSortType.split("|");
    var subjectNo = $("#hidSubjectNo").val();
    //  var ShowType = $("#sltShowType").val();
    // var ShowType = $("input[name='ridShowType']:checked").val();

    if (tempSortType == "2" || SortTypeArray[0] == "2") {
        var idList = "";
        var list_cell = $("#SortBrand li");
        if (list_cell != undefined) {
            for (var i = 0; i < list_cell.length; i++) {
                idList += list_cell[i].id + ",";
            }
        }
        if (tempSortType.indexOf("|") > 0) {
            tempSortType = tempSortType.substring(0, tempSortType.indexOf("|"));
        }
        tempSortType = tempSortType + "|" + idList;


        $("#hidSortType").val(tempSortType);

    }
    if (tempSortType == "7" || SortTypeArray[0] == "7") {
        var idList = "";
        var list_cell = $("#SortCategory li");
        if (list_cell != undefined) {
            for (var i = 0; i < list_cell.length; i++) {
                idList += list_cell[i].id + ",";
            }
        }
        if (tempSortType.indexOf("|") > 0) {
            tempSortType = tempSortType.substring(0, tempSortType.indexOf("|"));
        }
        tempSortType = tempSortType + "|" + idList;

        $("#hidSortType").val(tempSortType);

    }
    if (tempSortType == "8" || SortTypeArray[0] == "8") {
        var idList = "";
        var list_cell = $("#SortColor li");
        if (list_cell != undefined) {
            for (var i = 0; i < list_cell.length; i++) {
                idList += list_cell[i].id + ",";
            }
        }
        if (tempSortType.indexOf("|") > 0) {
            tempSortType = tempSortType.substring(0, tempSortType.indexOf("|"));
        }
        tempSortType = tempSortType + "|" + idList;

        $("#hidSortType").val(tempSortType);
    }
    if (tempSortType != "9" && tempSortType != "9|") {
        var _tempSortType = tempSortType.split("|");
        if (_tempSortType.length == 2) {
            if (_tempSortType[1] == null || _tempSortType[1] == "") {
                alert("请选择排序方式！");
                return false;
            }
        }
        else {
            alert("请选择排序方式！");
            return false;
        }
    }
    else {
        tempSortType += "|" + "9";
    }
    var _subjectNo = $("#hidsubjectNoN").val();

    var showType = $("#hidShowType").val();              // 展示渠道
    if (showType == "0")
        window.location.href = "/Outlet/Subject/ProductView?subjectNo=" + _subjectNo + "&SortType=" + tempSortType + "&SCategoryNo=" + subjectNo + "&IsAutoBottom=" + isAutoBottom + "&isOne=1&t=" + new Date().getTime()
    else
        window.location.href = "/Outlet/Subject/ProductMobileView?subjectNo=" + _subjectNo + "&SortType=" + tempSortType + "&SCategoryNo=" + subjectNo + "&IsAutoBottom=" + isAutoBottom + "&isOne=1&t=" + new Date().getTime()

}


function isOrderRad(code) {
    switch (code) {
        case "1|0":
            return "GD";
        case "1|1":
            return "DG";
        case "3|0":
            return "GD";
        case "3|1":
            return "DG";
        case "4|0":
            return "GD";
        case "4|1":
            return "DG";
        case "6|0":
            return "GD";
        case "6|1":
            return "DG";
        case "5|0":
            return "XJ";
        case "5|1":
            return "JX";
        default:
            return "";
    }
}


// 判断执行那个选项
function selectChange(value) {
    switch (value) {
        case "-1":
            OnChangeData('-1', '');
            break;
        case "1":
            OnChangeData('1', '1');
            break;
        case "2":
            OnChangeData('2', '3');
            break;
        case "3":
            OnChangeData('3', '1');
            break;
        case "4":
            OnChangeData('4', '1');
            break;
        case "5":
            OnChangeData('5', '2');
            break;
        case "6":
            OnChangeData('6', '1');
            break;
        case "7":
            OnChangeData('7', '4');
            break;
        case "8":
            OnChangeData('8', '5');
            break;
        case "9":
            OnChangeData('9', '-1');
            break;
    }
}
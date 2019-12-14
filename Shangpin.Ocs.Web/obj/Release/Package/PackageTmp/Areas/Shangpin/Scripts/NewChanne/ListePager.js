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
    if (confirm("您确定要删除吗？"));
    {
        var id = "";
        id = $(obj).parent().parent().children("td:first").text().split(',')[0]
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
    id = $(obj).parent().parent().children("td:first").text().split(',')[0];
    status = $(obj).parent().parent().children("td:first").next().next().next().text();
    if (status == "显示") {
        status = 0;
    }
    else {
        status = 1;
    }
    $.ajax({
        url: '/NewChannel/UpdateNaviState',
        type: "POST",
        data: { ID: id, Status: status },
        success: function (result) {
            if (result.Successed) {
                window.location = window.location.href;
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

//保存
function savetable() {
    var list = "";
    var i = 0;
    $("#table_main tr").each(function () {
        var text = $(this).children("td:first").text();
        list += i + "-";
        list += text + ",";
        i++;
    });

    $.ajax({
        url: '/NewChannel/Sorting',
        type: "POST",
        data: { Sort: list },
        success: function (result) {
            if (result.Successed) {
                alert("保存成功！");
                window.location = window.location.href;
            }
            else {
                alert("保存失败！");
                LG.showError('错误:' + result.Message);
            }
        },
        error: function (result) {
            LG.tip('发现系统错误 <BR>错误码：' + result.status);
        }
    });
    return false;
}

var i = 3;
$(document).ready(function () {
    $("#tabs li").bind("click", function () {
        var index = $(this).index();
        var divs = $("#tabs-body > div");
        $(this).parent().children("li").attr("class", "tab-nav");//将所有选项置为未选中
        $(this).attr("class", "tab-nav-action"); //设置当前选中项为选中样式
        divs.hide();//隐藏所有选中项内容
        divs.eq(index).show(); //显示选中项对应内容
    });

});

function AddManager() {
    $("#FormAdd").css("display", "block");
}

$(function () {
    $("#FormAdd").submit(function () {
        var linkname = $("#LinkName").val();
        if ($.trim(linkname) == "") {
            alert("名称不能为空！");
            return false;
        }
        else {
            var linknamelength = linkname.length;
            if (linknamelength > 10) {
                alert("名称限制在10个中文字符以内");
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
                    LG.showError('错误:' + result.Message);
                }
            },
            error: function (result) {
                LG.tip('发现系统错误 <BR>错误码：' + result.status);
            }
        });
        return false;
    })
})




function SaveListPagePrice() {
    var plist = "";
    $("input[p=price]").each(function (index, obj) {
        if ($.trim($(obj).val()) != "") {
            plist = plist + $(obj).val() + "-";
        }
    })
    if (plist != "") {
        plist = plist.substring(0, plist.length - 1);
    }
    $.ajax({
        url: '/NewChannel/AddsPrice',
        type: "POST",
        dataType: "text",
        data: { channelNo: $("#hidChannelNo").val(), priceList: plist },
        success: function (result) {
            alert("价格添加成功");
            $("#priceBody").html(result);
        },
        error: function (result) {
        }
    });
}

function GetPriceList() {
    $.ajax({
        url: '/NewChannel/AddsPrice?channelno=' + $("#hidChannelNo").val(),
        type: "POST",
        data: { channelNo: $("#hidChannelNo").val(), priceList: pricelist },
        success: function (result) {

        },
        error: function (result) {
        }
    });
}


function Add() {
    var td = $(":input[type=text]").last().val();
    var td1 = $(":input[type=text]").last().parent().prev().prev().text();

    if (td == "") {
        alert("必须填写结束价！");
        $(":input[type=text]").last().val("");
        return;
    }
    if (parseInt(td1) > parseInt(td)) {
        alert("结束价必须大于开始价！");
        $(":input[type=text]").last().val("");
        return;
    }
    var Isnan = false
    $("input[p=price]").each(function (index, obj) {
        if (isNaN($(obj).val())) {
            Isnan = true;
            $(obj).val("");
        }
    })
    if (Isnan) {
        alert("价格必须是数字");
        return;
    }
    $("input[p=price]").attr("readonly", "readonly");
    $("#tb1").append(" <tr  style='width:400px;height:40px;line-height:40px;text-align:center;'><td>" + td + "</td><td>-</td><td><input type='text' style='width:80px;height:30px' p='price' name='price" + (i++) + "'/></td></tr>");
}

/**************************************************************/
//======================运营属性===============================================
$(function () {
    $("#popel").click(function () {
        GetPopList();
    });
})


//获得列表
function GetPopList() {
    $("#popelebody").html("");
    $.ajax({
        url: '/NewChannel/GetLabelList?channelno=' + $("#hidsChannelNo").val() + "&rand=" + Math.random(),
        type: "Get",
        beforeSend: function (a, b, c) {
        },
        complete: function () {
        },
        success: function (result) {
            var html = "";
            $(result.Data).each(function (i, item) {
                var isShowStr = item.Status == "1" ? "显示" : "隐藏";

                html += "<tr style=\"height:25px; text-align:left; margin-left:20px;\" id=\"" + item.LabelNo + "\" nid='" + item.ElementsId + "' nsortid='" + item.SortId + "'>";
                html += "<td style=\"padding-left: 10px; background-color: #FFFFFF\">" + item.ParentName + "";
                html += "<span style=\"color:red; margin-left:5px; font-weight:bold\">(" + item.ProductCount + ")</span></td>";
                html += " <td style=\"padding-left: 10px; background-color: #FFFFFF; text-align: center\">";
                html += "<span style=\"cursor:pointer; color:blue;text-decoration:underline \" onclick=\"UpdatePopState('" + item.ParentNo + "','" + item.CategoryNo + "',this)\">" + isShowStr + "</span>";
                html += "</td>";
                html += "<td style=\"padding-left: 10px; background-color: #FFFFFF; text-align: center\"><span style=\"cursor:pointer;color:blue;\" onclick=\"GotoTopStyle(this)\">【上移】</span><span style=\"cursor:pointer;color:blue;\" onclick=\"GotoBottomStyle(this)\">【下移】</span></td>";
                html += "</tr>";
            })
            $("#popelebody").append(html);

        },
        error: function (result) {
        }
    })
}

function GotoTopStyle(obj) {
    var curobj = $(obj).parent().parent();
    if ($(curobj).index() > 1) {
        var newid = $(curobj).attr("nid");
        var newsortid = $(curobj).attr("nsortid");
        var oldid = $(curobj).prev().attr("nid");
        var oldsortid = $(curobj).prev().attr("nsortid");
        if (UpdateSort(newid, newsortid, oldid, oldsortid)) {
            $(curobj).prev().before(curobj);
        }
    }
}

function GotoBottomStyle(obj) {
    var curobj = $(obj).parent().parent();
    if ($(curobj).index() != ($(curobj).parent().find("tr").size() - 1)) {
        var newid = $(curobj).attr("nid");
        var newsortid = $(curobj).attr("nsortid");
        var oldid = $(curobj).next().attr("nid");
        var oldsortid = $(curobj).next().attr("nsortid");
        if (UpdateSort(newid, newsortid, oldid, oldsortid)) {
            $(curobj).next().after(curobj);
        }
    }
}

function UpdateSort(newid, newsortid, oldid, oldsortid) {
    var resulted = false;
    $.ajax({
        url: '/NewChannel/UpdateSort',
        type: "POST",
        async: false,
        dataType: "text",
        data: { newid: newid, newsortid: newsortid, oldid: oldid, oldsortid: oldsortid },
        success: function (result) {

            resulted = true;
        },
        error: function (result) {
        }
    })
    return resulted;
}

function UpdatePopState(labelNo, categoryNo, obj) {
    $.ajax({
        url: '/NewChannel/UpdateState',
        type: "Get",
        data: { "labelNo": labelNo, "categoryNo": categoryNo },
        success: function (result) {
            if ($(obj).html() == "显示") {
                $(obj).html("隐藏");
            } else {
                $(obj).html("显示");
            }
        },
        error: function (result) {
        }
    })
}
$(function () {
    if (parent.productContent && parent.picContent) {
        $("#productInfo").html(parent.productContent.html()); //从上级页面拿到商品信息 
        $("#picContent").html(parent.picContent.html());
    }
    $("#tbLabel tr:not(:last) select").live({
        change: function () {
            AddCloneSelect($(this));
        }
    })
    $(".deleteImg").live({
        click: function () {
            if (confirm("确定删除此标签类?")) {
                var id = $(this).parent().parent().attr("id");
                var text = $(this).parent().text();
                $("#allLabel").append("<option value='" + id + "'>" + $.trim(text) + "</option>");
                $(this).parent().parent().remove();
            }
        }
    })
    InitContent();
});
function InitContent() {//初始化选中项
    $("[pid]").each(function (i, n) {
        if ($(this).is("[normal=1]") || ($(this).is("[labeled]") && $(this).attr("labeled").length > 0)) {
            $("#allLabel").val($(this).attr("pid"));
            $("#allLabel").change();
            addLabelParnet($("#" + $(this).attr("pid")).find('select:last'));
            if ($(this).is("[labeled]") && $(this).attr("labeled").length > 0) {
                var ids = $(this).attr("labeled").split(',');
                for (var i in ids) {
                    AddCloneSelect($("#" + $(this).attr("pid")).find('select:last').val(ids[i]));
                }
            }
        }
    })
}
function AddCloneSelect(select) {//添加克隆下拉列表
    if ($(select).val().length > 0) {
        if ($(select).val() == "-1") {
            $(select).find("option:last").appendTo($(select).siblings().last());
            if (select.nextAll().length >=1) {
                select.nextAll().last().change();
            }
            $(select).remove();
        }
        else if (select.find("option").length > 2) {
            var clone = $(select).clone();
            clone.find("option").remove();
            clone.prepend("<option value='-1'>删除标签</option>");
            clone.append($(select).find("option:selected"));
            $(select).before(clone);
            $(select).find("option:selected:not([value=''])").remove();
            if ($(select).find("option").length == 1) { $(select).remove() }
        }
    }

}
function saveAndCloseWin() {
    if (!confirm("确定保存?")) return;
    var allIds = "";
    var labelParents = "";
    var labelParentNames = "";
    var labelNames = "";
    $("#tbLabel tr[id]").each(function (i, n) {
        var selectoptions = $(n).find("select option:selected:not([value=''])");
        if (selectoptions.length > 0) {
            labelParentNames += "," + $.trim($(n).children().eq(0).text());
            var ids = "";
            var names = "";
            selectoptions.each(function (j, m) {
                ids += ',' + $(m).val();
                names += "," + $(m).text();
            })
            allIds += "#" + ids.substring(1);
            labelNames += "##" + names.substring(1);
            labelParents += "," + $(n).attr("id");
        }
    })

    if (labelParentNames.length > 0)
        labelParentNames = labelParentNames.substring(1);
    if (labelNames.length > 0)
        labelNames = labelNames.substring(2);

    if (labelParents.length > 0)
        labelParents = labelParents.substring(1);
    if (allIds.length > 0)
        allIds = allIds.substring(1);
    // alert(labelParentNames + "___" + labelNames);
    // alert(allIds + "____________" + labelParents) 
    $.ajax({
        type: "post",
        url: "/Shangpin/ProductLabel/SaveProductRefLabel",
        data: { v: Math.random(), ids: allIds, pids: labelParents, productNo: $("#productNo").val() },
        success: function (result) {
            if (result && result.data == "1") {
                //alert("保存成功!");
                if (parent.setLabelDetail) {
                    parent.setLabelDetail(labelParentNames, labelNames);
                }
                CloseWin();
            }
            else if (result && result.msg) { alert(result.msg); }
        }
    })
}
function CloseWin() {
    top.$("#windownbg").remove();
    top.$("#windown-box").fadeOut("fast", function () { $(this).remove(); });
}
function addLabelParnet(select) {//添加标签类项
    var labelParentId = $(select).val();
    if (labelParentId.length == 0) return;
    var labelParentName = $(select).find("option:selected").text();
    var deleteImage = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
    if (!$("[pid=" + labelParentId + "]").is("[normal=1]")) {
        deleteImage = '<img class="deleteImg" src="/Areas/Outlet/Images/layout/delete.png" />';
    }
    var htmlstr = '<tr  id="' + labelParentId + '"><td class="rt">' + labelParentName + deleteImage + '</td><td class="lt">' + GetLabelSelect(labelParentId) + '</td></tr>';
    $("#tbLabel tr:last").before(htmlstr);
    $("#tbLabel tr").removeClass("cross");
    $("#tbLabel tr:even").addClass("cross");
    $(select).find("option:selected").remove();
}
function GetLabelSelect(labelParentId) {//生成标签类的下拉列表
    var val = $("[pid=" + labelParentId + "]").val();
    var htmlstr = "";
    if (val.length > 0) {
        var arr = val.split('||');
        var txts = arr[0].split(',');
        var ids = arr[1].split(',');
        var htmlstr = "<select>";
        htmlstr += "<option value=''>请选择标签</option>";
        for (var i = 0; i < ids.length; i++) {
            htmlstr += "<option value='" + ids[i] + "'>" + txts[i] + "</option>";
        }
        htmlstr += "</select>";
    }
    return htmlstr;

}

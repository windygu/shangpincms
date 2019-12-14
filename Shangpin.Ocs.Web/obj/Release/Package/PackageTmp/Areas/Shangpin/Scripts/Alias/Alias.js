
function AliasSave(brandNo, typeId, gender) {
    var alias = $("#" + brandNo).val();
    $.ajax({
        url: "/shangpin/seo/AjaxSaveBrandAlias.html",
        type: "POST",
        data: { brandNo: brandNo, alias: alias, typeId: typeId, gender: gender },
        dataType: "json",
        success: function (obj) {
            if (obj.result > 0)
                alert("保存成功");
            else if (obj.result == -1) {
                alert("该别名已存在");
                $("#" + brandNo).val("");
            }
            else
                alert("保存失败");
            //window.location.reload(force = true);
            window.location.href = gettimestampurl(window.location.href);
        }

    });
}
function AliasDelete(id) {
    $.ajax({
        url: "/shangpin/seo/AjaxDeleteBrandAlias.html",
        type: "POST",
        data: { id: id },
        dataType: "json",
        success: function (obj) {
            if (obj.result > 0)
                alert("删除成功");
           
            else
                alert("删除失败");
            window.location.href = gettimestampurl(window.location.href);
        }

    });
}


function brandserach() {
    var brandName = $("#brandName").val();
    var aliasName = $("#aliasName").val();
    window.location.href = gettimestampurl("/shangpin/seo/brandalias.html?brandname=" + escape($.trim(brandName)) + "&aliasName=" + escape($.trim(aliasName)));

}

function categoryserach() {
    var categoryName = $("#categoryName").val();
    var aliasName = $("#aliasName").val();
    window.location.href = gettimestampurl("/shangpin/seo/CategoryAlias.html?CategoryNo=" + $("#CategoryNo").val() + "&categoryName=" + escape($.trim(categoryName))+"&aliasName=" + escape($.trim(aliasName)));
}
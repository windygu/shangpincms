/*
 Des:活动创建时关联品牌的处理---已不使用
*/
$(function () {
    $("#OutletSubjectSpike select").find("option:selected").text("下拉单选品牌列表");
    $("#barndView input[value='确定']").bind("click", checkBrandNo);
    var brandNo = $("#OutletSubjectBrandNo").val();
    if (brandNo.length > 0 && brandNo != "noBrandNo" && brandNo != "moreBrandNo") {
        $("#barndView input[name='ck_Brand']").each(function () {
            if (this.value == brandNo) {
                this.checked = true;
            }
        });
    }
});
function checkBrandNo() {
    if ($("#customSelect")) {
        $("#customSelect").hide();
    }
    var txt = $("#subBrandAllName").text();
    if (txt.length <= 0) {
        alert("请选择活动所属品牌");
        $("#OutletSubjectBrandNo").val("noBrandNo"); //没有关联品牌
        return false;
    }
    var num = 0;
    $("#barndView input[name='ck_Brand']:checked").each(function () {
        if (this.checked) {
            $("#OutletSubjectBrandNo").val(this.value);
            num++;
        }
    });
    if (num == 1) {
        return true;
    }
    else {
        alert("活动只能关联一个品牌");
        $("#OutletSubjectBrandNo").val("moreBrandNo"); //提示关联品牌太多
        return false;
    }
}
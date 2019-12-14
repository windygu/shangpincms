$(function () {
    if ($("#PositionParentId").val() == "1" || ($("#PositionParentId").val() == "0" && $("#PositionId").val() == "1")) {
        $("#womentwo").show();
        $("#mentwo").hide();
    }
    else if ($("#PositionParentId").val() == "2" || ($("#PositionParentId").val() == "0" && $("#PositionId").val() == "2")) {
        $("#mentwo").show();
        $("#womentwo").hide();
    }
    var category = $("#PositionParentId").val();
    category = category == "0" ? $("#PositionId").val() : category;
    $("#categoryone").val(category);
    if (category == "1") {
        $("#womentwo").val($("#PositionId").val());
    }
    else if (category == "2") {
        $("#womentwo").hide();
        $("#mentwo").show();
        $("#mentwo").val($("#PositionId").val());
    } else {
        $("#mentwo").hide();
        $("#womentwo").hide();
    }
});
function CategoryOneChange() {
    var category = $("#categoryone").val();
    $("#PositionParentId").val("0");
    $("#PositionId").val(category);

    if (category == "1") {
        $("#womentwo").show();
        $("#mentwo").hide();
        $("#womentwo").val("0");
    }
    else
        if (category == "2") {
            $("#mentwo").show();
            $("#womentwo").hide();
            $("#mentwo").val("0");
        }
        else {
            $("#womentwo").hide();
            $("#mentwo").hide();
        }
}

function CategoryTwoChange() {
    var category = $("#categoryone").val();
    var categoryTwo;
    if (category == "1") {
        categoryTwo = $("#womentwo").val();
    }
    else {
        categoryTwo = $("#mentwo").val();
    }
    if (categoryTwo == "0") {
        $("#PositionParentId").val("0");
        $("#PositionId").val(category);
    }
    else {
        $("#PositionParentId").val(category);
        $("#PositionId").val(categoryTwo);
    }
}
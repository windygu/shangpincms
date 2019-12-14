$(function () {

    $("#LoginBtn").bind("click", function () {
        var UserName = $("#formSignIn_UserName").val();
        var Password = $("#formSignIn_Password").val();
        if (UserName.length <= 0) {
            $("#formSignIn_UserName_msg").text("用户名不能为空");
            return false;
        }
        if (Password.length <= 0) {
            $("#formSignIn_Password_msg").text("密码不能为空");
            return false;
        }
        if ($("#RemberOCSUser").attr("checked") == "checked") {
            $("#RememberMe").val("1");
        }
        $("#formSignIn").submit();
    });

});
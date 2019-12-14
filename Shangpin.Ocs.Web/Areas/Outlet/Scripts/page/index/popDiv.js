/*rightcontent handel*/
//弹出对话框
function PopWindowShow() {
    //alert("222");
    var isClick = $(this).attr("class");
    //alert(isClick);
            var $window$ = $("#mx-rightcontent-fixbarbg");
            var $nav$ = $("#mx-rightcontent-fixbar");
            var mianH = parseInt($(window).height() - 154),
				        topH = parseInt(mianH - 79);
            $($window$).css({ "height": mianH, "overflow-y": "scroll" });
            //$($nav$).css({ "bottom": topH });
}

//关闭对话框
function PopWindowClose(){
            var $window$ = $("#mx-rightcontent-fixbarbg");
            var $nav$ = $("#mx-rightcontent-fixbar");
            $($window$).css({ "height": 79, "overflow-y": "hidden" });
            //$($nav$).css({ "bottom": 0 });
        }

        $(function () {

            $("#mx-rightcontent-fixbar a").click(function () {

                PopWindowShow();
            });
        });
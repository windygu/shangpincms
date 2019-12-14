/**
 * Created with JetBrains WebStorm.
 * User: MacroXing
 * Date: 13-6-8
 * Time: 下午5:24
 * To change this template use File | Settings | File Templates.
 */
(function (window, $, undefined) {
    var imgSwitch = (function () {
        var imgqueue = [
                '/images/ex/img2.jpg',
                '/images/ex/img0.jpg',
                '/images/ex/img1.jpg',
                '/images/ex/img3.jpg',
                '/images/ex/img4.jpg'
            ],
            indexerStyle = function (index) {
                //设置图片区域的超链接
                $("a[mximg-indexer]").removeClass("cur");
                var cur = $("a[mximg-indexer='" + index + "']");
                cur.addClass("cur");
            };

        $(function () {
            var MXSwitcher = $.MXImage.Switchover({
                imgqueue: imgqueue,
                wrapper: $("#mx-index-rollimg"),
                preload: function (structor, switcher) {
                    if (!switcher.last) {
                        var last = switcher.last = this.last();
                        if (structor.status !== 2) {
                            switcher.wrapper.removeClass("loaderhide");
                            if (last && last.img && last.img.data("dom")) {
                                last.img.data("dom").animate({opacity: 0.5}, 300);
                            }
                            else {
                                last.img.animate({opacity: 0.5}, 300);
                            }
                        }
                        else {
                            switcher.wrapper.addClass("loaderhide");
                        }
                    }
                    indexerStyle(this.index());
                },
                onload: function (structor, switcher) {
                    var imgdom = $("<a class='imgcontent' href='#' style='background-image:url(" + structor.img.attr("src") + ")'></a>").hide();
                    structor.img.data("dom", imgdom);
                    switcher.wrapper.prepend(imgdom);
                },
                always: function (structor, switcher) {
                    var currentIndex = this.index(),
                        last = switcher.last;
                    if (structor.index === currentIndex) {
                        if (last && last.status === 2) {
                            if (last.img.data("dom")) {
                                last.img.data("dom").stop(true, true).fadeOut(600);
                            }
                            else {
                                last.img.stop(true, true).fadeOut(600);
                            }
                            switcher.last = null;
                        }
                        if (structor.img.data("dom")) {
                            structor.img.data("dom").css("opacity", 1).fadeIn(1000);
                        }
                        else {
                            structor.img.css("opacity", 1).fadeIn(1000);
                        }
                        switcher.autostart();
                    }
                },
                auto: 5000
            });

            var firstSwitchChild = $("#mx-index-rollimg a#firstcontent");
            firstSwitchChild.data("dom", firstSwitchChild);
            MXSwitcher.replace(firstSwitchChild, 0);
            MXSwitcher.current(0, {
                preload: function () {
                },
                onload: function (structor, switcher) {
                    structor.img.data("dom", firstSwitchChild);
                },
                always: function (structor, switcher) {
                    switcher.autostart();
                }
            });

            $("a[mximg-indexer]").on("mouseenter", function (e) {
                var $indexer = parseInt($(this).attr("mximg-indexer"));

                if(!isNaN($indexer)){
                    MXSwitcher.current($indexer);
                }

                return false;

            });

            $("#mx-index-rollimg").hover(
                function(){
                    MXSwitcher.autostop();
                },
                function(){
                    //这里要用restart方法
                    MXSwitcher.autorestart();
                }
            );
        });

    })();
})(window, jQuery)
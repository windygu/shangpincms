/**
 * Created with JetBrains WebStorm.
 * User: MacroXing
 * Date: 13-7-20
 * Time: 下午3:33
 * To change this template use File | Settings | File Templates.
 */
;
(function (window, $, MXTempl, undefined) {
    $(function () {
        var postdata = {
            imgurl: null,
            href: null
        };
        var brfsWindow = $brfsWindow({
            binder: function () {
                var _this = this;
                $("#sp_brfs_edit_imgfile").uploadify({
				    "swf"      : "/Areas/Shangpin/Scripts/js/uploadify.swf",
				    "uploader" : "/Shangpin/Brand/GetImgPath.html",
                    onUploadSuccess: function(file, data, response){
                       postdata.imgurl = data;
                       //alert(data);
                    },
                    onUploadError: function(file, errorcode, errormsg, errorstr){
                        console.log(errorcode);
                        console.log(errormsg);
                        console.log(errorstr);
                    }
			    });

                $("#sp_brfs_edit_submit").on("click", function () {
                    postdata.href = $("#sp_brfs_edit_imgurl").val();
                    $.ajax(
                        {
                            url: "/Shangpin/Brand/GetAjaxImgJson.html",
                            type: "POST",
                            data: {'picPath':postdata.imgurl,'imglink':postdata.href, 'targetlabel':MXTempl("sp_brfs").operator},
                            dataType: "JSON",
                            cache: false
                        }
                    ).done(
                        function (data) {
                            if (!data) {
                                return;
                            }
                            var imgsrc = data.imgsrc,
                                imghref = data.imghref,
                                targetlabel = data.targetlabel,
                                htmlcontent = "<a href='" + imghref + "' target='_blank'><img src='" + imgsrc + "' /></a>";

                            $("span[templ_label='" + targetlabel + "']").html(htmlcontent);

                            MXTempl("sp_brfs").update(targetlabel, htmlcontent, $("#template_test_wrapper").html());
                            //console.log(MXTempl("sp_brfs").html);
                            //alert($("#sp_brfs_inner").html());
                            $("#HtmlCode").html(MXTempl("sp_brfs").html);
                            _this.close();
                        }
                    ).fail(
                        function () {
                            _this.close();
                        }
                    ).always(function(){
                        postdata.imgurl = null;
                        postdata.href = null;
                    });
                    return false;
                });
            }
        });
        
        MXTempl("sp_brfs")
            .requestData("/Shangpin/Brand/GetTemplateText.html")///data/templ_example.txt
            .renderReady(function () {
                $("#template_test_wrapper").html(this.html);
                $(document).delegate("span[templ_label] a", "click", function () {
                    return false;
                });
                $("span[templ_label],").on("click", function () {
                    var $this = $(this),
                        $imgobj = $this.find("img"),
                        $hrefobj = $this.find("a"),
                        $imgsrc = $imgobj.length > 0 ? $imgobj.attr("src") : null,
                        $hrefsrc = $hrefobj.length > 0 ? $hrefobj.attr("href") : null;

                    brfsWindow.show(function (handler) {
                        var imgfile = $("#sp_brfs_edit_imgfile"),
                            href = $("#sp_brfs_edit_imgurl");
                        imgfile.prev("img").hide()
                        href.val("");
                        if ($imgsrc) {
                            imgfile.prev("img").attr("src", $imgsrc).show();
                        }
                        if ($hrefsrc) {
                            href.val($hrefsrc);
                        }
                        handler.position();
                    });
                });
            });
    });
})(window, jQuery, MXTempl);

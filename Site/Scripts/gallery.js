$(function() {
    function prepareWall() {
        $(".wall a").click(function() {
            $(this).parent().toggleClass('selected');
        })
        .fancybox({
            padding	        : 0,
            transitionIn	: 'fade',
            transitionOut	: 'fade',
            overlayOpacity  : 0.8,
            overlayColor    : '#000',
            titlePosition   : 'over',
            titleFormat     : function(title, currentArray, currentIndex) {
                var a = currentArray.eq(currentIndex);
                return "<span id='fancybox-title-over' class='image-actions'>" +
                    "<a href='" + a.attr('data-action-download') + "'>Download</a>" +
                    "<a href='" + a.attr('data-action-comment') + "'>Comment</a>"
                "</span>";
            }
        });

        var imgs = $(".wall img");
        imgs.lazyload({
            placeholder : imgs.eq(0).attr('src'),
            container   : $("#main"),
            effect      : "fadeIn"
        });

        $('.wall').dragToSelect({
            selectables  : '.item',
            selectOnMove : true/*,
            onHide       : function () {
                var selected = $('.wall .item.selected');
                if (selected.length > 0) {
                    $("#main").css('right', '12.6em');
                    $("#right-menu").show();
                }
                else {
                    $("#right-menu").hide();
                    $("#main").css('right', '0');
                }
            }*/
        });
    }

    prepareWall();

    $("#left a.album-name").click(function(event) {
        event.preventDefault();

        var that = $(this);
        $.get(that.attr('href'), {}, function(html) {
            $('#main').html(html);

            that.parent().find('.selected').removeClass('selected');
            that.addClass('selected');

            prepareWall();

            document.title = document.title.replace(/:.+$/, "") + ": " + that.text();
        });
    });

    $("#left a.more").click(function(event) {
        event.preventDefault();

        var that = $(this);
        $.get(that.attr('data-ajax'), {}, function(html) {
            that.replaceWith(html);
        });
    });    
});
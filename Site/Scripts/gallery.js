function setupSecurityPanel() {
    $("a.album-visible-to").fancybox({
        transitionIn	: 'fade',
        transitionOut	: 'fade',
        overlayOpacity  : 0.8,
        overlayColor    : '#000',
        onComplete      : function() {
            $('form.grant img')
                .click(function() {
                    var group = $(this).parent();

                    group.toggleClass('granted');
                    var hidden = group.find('input');
                    hidden.val(hidden.val() == "" ? group.attr('data-key') : "");
                });

            $('form.grant').ajaxForm({
                success : function() {
                    $.fancybox.close();
                    loadAlbum($("#left a.album-name.selected")); // reloads album
                }
            });
        }
    });
}

function setupAlbum() {
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

    $("#main").scrollTop(0);
    var imgs = $(".wall img");
    imgs.lazyload({
        placeholder : imgs.eq(0).attr('src'),
        container   : $("#main"),
        effect      : "fadeIn"
    });

    setupSecurityPanel();

    /*$('.wall').dragToSelect({
        selectables  : '.item',
        selectOnMove : true/ *,
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
        }* /
    });*/
}

function loadAlbum(a) {
    $.get(a.attr('href'), {}, function(html) {
        $('#main').html(html);

        a.parent().find('.selected').removeClass('selected');
        a.addClass('selected');

        setupAlbum();

        document.title = document.title.replace(/:.+$/, "") + ": " + a.text();
    });        
}

$(function() {
    setupAlbum();

    $("#left a.album-name").live('click', function(event) {
        event.preventDefault();
        loadAlbum($(this));
    });

    $("#left a.more").live('click', function(event) {
        event.preventDefault();

        var a = $(this);
        $.get(a.attr('data-ajax'), {}, function(html) {
            a.replaceWith(html);
        });
    });    
});
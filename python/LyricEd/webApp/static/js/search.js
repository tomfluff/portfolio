$(document).ready(function () {

    // search page
    // -----------------
    $('#search-button').on('click', function () {
        // search-term, difficulty-level, tag-select, select-year, length-bound
        var sch = $('#search-term input').val();
        var diff = $('#difficulty-level label.active input').attr('data-value');
        var tgs_arr = [];
        console.log("testing log");
        $('.tag-item.active').each(function (index, element) {
            var tg_txt = $(this).attr('data-value').split(' ').join('+');
            console.log(index + ' ' + tg_txt);
            tgs_arr.push($.trim(tg_txt));
        });
        var tgs = tgs_arr.join(',');
        var yr = $('#select-year').val();
        var len = $('#length-bound label.active input').attr('data-value');
        location.replace('/search?diff=' + diff + '&sch=' + sch + '&tgs=' + tgs + '&yr=' + yr + '&len=' +  len)
    });

    $(document).on('click', '.song-buttons .btn-success', function () {
        var sg = $(this).attr('data-song');
        var diff = $('#difficulty-level label.active input').attr('data-value');
        if (diff === '-1') {
            diff = 1;
        }
        location.replace('/play?sg=' + sg + '&diff=' + diff);
    });

    $(document).on('click', '.song-buttons .song-keep', function () {
        var song_id = $(this).attr('data-song');
        var user_id = $(this).attr('data-user');
        var btn = $(this);

        if (!btn.hasClass('btn-warning')) {
            $.post($SCRIPT_ROOT + '/add_keep', {us: user_id, sg: song_id}, function (data) {
                if (data['status'] === 'OK') {
                    btn.removeClass('btn-default');
                    btn.addClass('btn-warning');
                }
                console.log(data)
            }, 'json');
        } else {
            $.post($SCRIPT_ROOT + '/remove_keep', {us: user_id, sg: song_id}, function (data) {
                if (data['status'] === 'OK') {
                    btn.removeClass('btn-warning');
                    btn.addClass('btn-default');
                }
                if (data['status'] === 'ERROR') {
                }
                console.log(data)
            }, 'json');
        }
    });

    // -- end search page
});
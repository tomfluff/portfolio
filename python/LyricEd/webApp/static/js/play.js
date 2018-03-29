$(document).ready(function () {
    /* after the page and all the components load */

    // for play page
    // -----------------
    $('[data-toggle="tooltip"]').tooltip();

    $('#report-modal').on('show.bs.modal', function (event) {
        // Button that triggered the modal
        var button = $(event.relatedTarget);
        // Extract info from data-* attributes
        var recipient = button.data('song');
        // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
        // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
        var modal = $(this);
        modal.find('.modal-body input').val(recipient);
    });

    // when clicking 'submit'
    $('#submit-song-btn').on('click', function () {
        var corr = 0;
        var incorr = 0;
        var song_id = $('#report-btn').attr('data-song');
        $('.missing-word').each(function (index, element) {
            if ($(this).val() === $(this).attr('data-word')) {
                // correct
                corr += 1;
                $('#mistake-display').append('<li class="bg-success"><strong>' + $(this).attr('data-word') + '</strong></li>');
            } else {
                // incorrect
                incorr += 1;
                $('#mistake-display').append('<li class="bg-danger">' + $(this).val() + ' &rarr; <strong>' + $(this).attr('data-word') + '</strong></li>');
            }
        });
        var total = corr + incorr;
        $('#num-mistakes').text(corr.toString() + '/' + total.toString()).attr('data-value',incorr);
        var score = Math.round(100 * corr / parseFloat(total));
        $('#user-score').text(score.toString() + '%').attr('data-value',score);
        $.getJSON($SCRIPT_ROOT + '/avg_score?sg=' + song_id, function (data) {
            $('#avg-score').text(data + '%');
        });
    });

    // when clicking continue
    $('#result-continue-btn').on('click', function () {
        console.log('continue clicked');
        var song_id = $(this).attr('data-song');
        var user_id = $(this).attr('data-user');
        var mistakes = $('#num-mistakes').attr('data-value');
        var score = $('#user-score').attr('data-value');

        $.post($SCRIPT_ROOT + '/add_history', {us:user_id,sg:song_id,mk:mistakes,sc:score}, function (data) {
            if (data['status'] === 'OK') {
                // no problems
            }
            console.log(data)
        }, 'json');
        // choose where to continue
        if ($(this).attr('data-next')) {
            var next_id = $(this).attr('data-next');
            var diff = $(this).attr('data-diff');
            location.replace($SCRIPT_ROOT + '/play?sg=' + next_id + '&diff=' + diff);
        }
        else {
            location.replace($SCRIPT_ROOT + '/search');
        }
    });

    // when clicking return
    $('#return-btn').on('click', function () {
        console.log('return clicked');
        location.replace($SCRIPT_ROOT + '/search');
    });

    // when clicking retry
    $('#result-retry-btn').on('click', function () {
        console.log('retry clicked');
        location.replace(location.href);
    });

    // when typing a tag
    $('#add-tag-text').keyup(function () {
        $('#tag-error-block').addClass('hidden');
    });

    // when adding tag
    $('#add-tag-btn').on('click', function () {
        var song_id = $(this).attr('data-song');
        var tag = $('#add-tag-text').val();
        console.log(song_id + ' - ' + tag);
        $.post($SCRIPT_ROOT + '/add_tag', {tg: tag, sg: song_id}, function (data) {
            if (data['status'] === 'OK') {
                $('#tag-list ul').append('<li><button type="button" class="delete-tag close" data-tag="' + tag + '">&times;</button><span>' + tag + '</span></li>');
            }
            if (data['status'] === 'EXISTS') {
                $('#tag-error-block').removeClass('hidden');
            }
            $('#add-tag-text').val('');
            console.log(data)
        }, 'json');
    });

    // delete tag
    $(document).on('click', '.delete-tag', function () {
        var song_id = $('#add-tag-btn').attr('data-song');
        var tag = $(this).attr('data-tag');
        $.post($SCRIPT_ROOT + '/remove_tag', {tg: tag, sg: song_id}, function (data) {
            if (data['status'] === 'OK') {
                var tag_item = '.delete-tag[data-tag="' + tag + '"]';
                console.log(tag_item);
                $(tag_item).parent().remove();
            }
            console.log(data)
        }, 'json');
    });

    $(document).on('click', '#difficulty-level .btn:not(.active)', function () {
        var difficulty = $(this).children('input').attr('data-value');
        var song_id = $('#add-tag-btn').attr('data-song');
        location.replace($SCRIPT_ROOT + '/play?sg=' + song_id + '&diff=' + difficulty);
    });

    $('#song-keep').on('click', function () {
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
    // -- end play page
});
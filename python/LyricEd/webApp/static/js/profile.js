$(document).ready(function () {

    $(document).on('click','.keep-list-play', function () {
        var sg = $(this).attr('data-song');
        location.replace('/play?sg=' + sg + '&diff=1');
    });

    $(document).on('click', '#logout-button', function () {
       $.post($SCRIPT_ROOT + '/logout' , function (data) {
            if (data['status'] === 'OK') {
                location.replace(data['url']);
            }
            if (data['status'] === 'ERROR') {
                location.replace(data['url']);
            }
            console.log(data)
        }, 'json');
    });

    $(document).on('click', '.keep-list-remove', function () {
        var song_id = $(this).attr('data-song');
        var user_id = $(this).attr('data-user');
        var row = $(this).closest('tr');
        $.post($SCRIPT_ROOT + '/remove_keep',{us:user_id, sg:song_id} , function (data) {
            if (data['status'] === 'OK') {
                row.fadeOut(500);
            }
            if (data['status'] === 'ERROR') {
            }
            console.log(data)
        }, 'json');
    });
});
// Dodawanie znajomego do pokoju.

// ukrywanie formularza do zapraszania przyjaciół
$("#addFriend").hide();

// wysyłanie zaproszenia
$('#addFriendButton').click(function () {
    var friendname = $('#friendName').val();
    $('#addFriendButton').attr("disabled", true);
    $('#sendedRoomInvitation').text('Wysyłanie zaproszenia...');
    var myroom = location.search && location.search.split('?')[1];

    $.ajax({
        url: 'https://' + location.host + '/Friends/inviteToRoom',
        type: 'POST',
        data: { username: friendname, roomname: myroom },
        success: function (data) {
            $('#sendedRoomInvitation').text(data);
            $('#addFriendButton').attr("disabled", false);
        }
    });
    $('#friendName').val('').focus();
});

// sprawdzanie zaproszeń do pokojów
var interval = 3000;  // 3 sekundy
function checkRoomInvitations() {
    $.ajax({
        type: 'GET',
        url: 'https://' + location.host + '/Friends/roomInvite',
        success: function (data) {
            //console.log(data);
            $('#notifications').html('<h3>Powiadomienia</h3>' + data);
        },
        complete: function (data) {
            // zaplanowanie kolejnego sprawdzenia
            setTimeout(checkRoomInvitations, interval);
        }
    });
}
setTimeout(checkRoomInvitations, interval);

$('#notifications').on('click', '#deleteInvitationButton', function () {
    $.ajax({
        type: 'POST',
        url: 'https://' + location.host + '/Friends/delRoomInvite',
        data: { roomname: this.value },
        success: function (data) {
            console.log("Usunięto zaproszenie do pokoju "+this.value);
        }
    });
});
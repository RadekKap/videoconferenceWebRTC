// Dodawanie znajomego do pokoju.

// Zmienne pomocnicze
var hostname = location.host;

// ukrywanie formularza do zapraszania przyjaciół
$("#addFriend").hide();

// wysyłanie zaproszenia
$('#addFriendButton').click(function () {
    var friendname = $('#friendName').val();
    $('#addFriendButton').attr("disabled", true);
    $('#sendedRoomInvitation').text('Wysyłanie zaproszenia...');
    var myroom = location.search && location.search.split('?')[1];

    $.ajax({
        url: 'https://' + hostname + '/Friends/inviteToRoom',
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
        url: 'https://' + hostname + '/Friends/roomInvite',
        success: function (data) {
            console.log(data);
            $('#notifications').html('<h3>Powiadomienia</h3>' + data);
        },
        complete: function (data) {
            // zaplanowanie kolejnego sprawdzenia
            setTimeout(checkRoomInvitations, interval);
        }
    });
}
setTimeout(checkRoomInvitations, interval);

// usuwanie zaproszenia do pokoju na żądanie zapraszanego
$('#notifications').on('click', '#deleteInvitationButton', function () {
    var roomname = this.value;
    $('#roomInvite'+roomname).hide();
    $.ajax({
        type: 'POST',
        url: 'https://' + hostname + '/Friends/delRoomInvite',
        data: { roomname: roomname },
        success: function () {
            console.log("Usunięto zaproszenie do pokoju " + roomname);
            //$('#deleteInvitationButton').show();
            checkRoomInvitations();
        }
    });
});
﻿// Dodawanie znajomego do pokoju.

// Zmienne pomocnicze
var hostname = location.host;

// ukrywanie formularza do zapraszania przyjaciół i listy znajomych
$("#addFriend").hide();
$("#friendsTable").hide();

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
var interval = 5000;  // 5 sekundy
var lastData; // zmienna pomicnicza przy aktualizacji danych
function checkRoomInvitations() {
    $.ajax({
        type: 'GET',
        url: 'https://' + hostname + '/Friends/roomInvite',
        success: function (data) {
            if (data != lastData) {
                console.log(data);
                $('#notifications').html('<h3>Powiadomienia</h3>' + data);
                lastData = data;
            }
        },
        complete: function (data) {
            // zaplanowanie kolejnego sprawdzenia
            setTimeout(checkRoomInvitations, interval);
        }
    });
}

// pierwsze sprawdzenie jest wymuszone tym kodem
checkRoomInvitations();

// usuwanie zaproszenia do pokoju na żądanie zapraszanego
$('#notifications').on('click', '#deleteInvitationButton', function () {
    var roomname = this.value;
    $('#roomInvite' + roomname).hide();
    $.ajax({
        type: 'POST',
        url: 'https://' + hostname + '/Friends/delRoomInvite',
        data: { roomname: roomname },
        success: function () {
            console.log("Usunięto zaproszenie do pokoju " + roomname);
        }
    });
});

$('#friendsTable').on('click', '#addThisFriend', function () {
    var friendname = this.value;
    var myroom = location.search && location.search.split('?')[1];
    $(this).attr("disabled", true);

    $.ajax({
        url: 'https://' + hostname + '/Friends/inviteToRoom',
        type: 'POST',
        data: { username: friendname, roomname: myroom },
        success: function () {
            // TODO: poinformowanie użytkownika lub usunięcie krotki
        },
        error: function () {
           $(this).attr("disabled", false);
        }
    });
});
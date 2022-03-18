<?php


    $con = mysqli_connect('localhost', 'root', 'root', 'thegameofroskilde', 3306);

    if(mysqli_connect_errno()){
        echo "1: Connection failed";
        exit();
    }

    $username = $_POST["username"];
    $password = $_POST["password"];

    //check if name exist

    $namecheckquery = "SELECT username FROM players WHERE username='" . $username . "';";

    $namecheck = mysqli_query($con, $namecheckquery) or die("2: Namecheck failed");

    if(mysqli_num_rows($namecheck) > 0){
        echo "3: Playername already exist";
        exit();
    }

    $salt = "\$5\$rounds=5000\$" . "steamedhams" . $username . "\$";
    $hash = crypt($password, $salt);

    $insertuserquery = "INSERT INTO players (username, hash, salt) VALUES ('" . $username . "', '" . $hash . "', '" . $salt . "'); ";

    mysqli_query($con, $insertuserquery) or die("4: insert failed");
 
    echo "0";

    exit();



?>  



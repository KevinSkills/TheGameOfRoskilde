<?php

    
    $con = mysqli_connect('localhost', 'id18805897_kevinskills', 'AntonOgKevin123.', 'id18805897_connectiondb', 3306);

    if(mysqli_connect_errno()){
        echo "1: Connection failed";
        exit();
    }
    

    $playerID = $_POST["playerID"];
    $port = $_POST["port"];
    $ip = $_POST["ip"];

    
    //check if name exist

    $playerIDcheckquery = "SELECT playerID FROM players WHERE playerID='" . $playerID . "';";

    $playerIDcheck = mysqli_query($con, $playerIDcheckquery) or die("2: Namecheck failed");

    

    if(mysqli_num_rows($playerIDcheck) > 0){

        $updateDataQuery = "UPDATE players
        SET port = '" . $port . "', ip = '" . $ip . "'
        WHERE playerID = '" . $playerID . "';
        ";

        mysqli_query($con, $updateDataQuery) or die("3: Update variable failed");

        echo "0";
        
        exit();
    }

    //salt og hash er egentlig ligemeget burde slette
    $salt = "\$5\$rounds=5000\$" . "steamedhams" . $playerID . "\$";
    $hash = crypt("password", $salt);

    $insertuserquery = "INSERT INTO players (playerID, hash, salt, port, ip) VALUES ('" . $playerID . "', '" . $hash . "', '" . $salt . "', '" . $port . "', '" . $ip . "'); ";

    mysqli_query($con, $insertuserquery) or die("4: insert failed");
 
    echo "0";

    exit();



?>  



<?php
    $students_id = $_SESSION['studentID'];
?>

<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="homepageStyle.css">
    <link rel="stylesheet" href="pop-up-post.css">
    <link rel= "icon" href="icon/brand-logo.png" type="icon/brand-logo.png">
    <title>Home Page</title>
</head>

<!-- Friend/Group Listing -->
        <aside id = "right-content">
            <div id = "xtau">
                <button href=" " class = "right-button friend" id="fbutton">Friend</button>
                <button href=" " class = "right-button group" id="gbutton">Group</button>
            </div>
            <div id = "friend-container" class="list-container">
                <?php
                    $sql = "SELECT studentID,name, status, profilePic 
                            FROM students 
                            WHERE studentID IN (
                            SELECT 
                            CASE 
                            WHEN studentID_1 = '$students_id' THEN studentID_2
                            WHEN studentID_2 = '$students_id' THEN studentID_1
                            END
                            FROM friendship
                            WHERE (studentID_1 = '$students_id' OR studentID_2 = '$students_id') AND friendReq = 2)";
                    $result = mysqli_query($conn, $sql);
                    if (mysqli_num_rows($result) > 0) { // check row
                        while($row = mysqli_fetch_assoc($result)) {

                            echo "<a href='#' class='in-profile'>";
                            if ($row['profilePic'] != null) { 
                                $profilePic = base64_encode($row['profilePic']); // convert blob to base64
                                echo '<img src="data:image/jpeg;base64,' . $profilePic . '" alt="side icon" class="profile-icon">';
                            } else { 
                                echo '<img src="icon/profile-icon.png" alt="side icon" class="profile-icon">'; // display default image if student not set profile pic
                            }
                            // if($row['status'] == "online"){
                            //     echo "<span class='in-profile-text'>" . $row['name'] . "<img id='status' src='icon/online-icon.png' alt='Online Status'/></span></a>";
                            // }else if($row['status'] == "offline"){
                            //     echo "<span class='in-profile-text'>" . $row['name'] . "<img id='status' src='icon/offline-icon.png' alt='Offline Status'/></span></a>";
                            // }

                            if($row['status'] == "online"){
                                echo "<span class='in-profile-text'>" . $row['name'] . "<img id='status' src='./icon/online-icon.png' alt='Online Status'/></span></a>";
                            }else if($row['status'] == "offline"){
                                echo "<span class='in-profile-text'>" . $row['name'] . "<img id='status' src='./icon/offline-icon.png' alt='Offline Status'/></span></a>";
                            }
                            // echo "<span class='in-profile-text'>" . htmlspecialchars($row['name']) . "<p>" "</p></span></a>";
                            echo "<script>console.log('". $row['name'] .": " . $row['status'] . "');</script>";

                        }
                    }
                ?>
            </div>

            <div id = "group-container"  class="list-container">
            <?php

$sql_group = "SELECT g.groupName, g.groupImage FROM `students` s JOIN `group_member` gm ON s.studentID = gm.studentID JOIN `group` g ON gm.groupID = g.groupID WHERE s.studentID = '$students_id'";

$result_group = mysqli_query($conn, $sql_group);

if (mysqli_num_rows($result_group) > 0) {   
    while($row_group = mysqli_fetch_assoc($result_group)) {
        echo "<a href='#' class='in-profile'>";
        if ($row_group['groupImage'] != null) {
            $groupImage = base64_encode($row_group['groupImage']);
            echo '<img src="data:image/jpeg;base64,' . $groupImage . '" alt="side icon" class="profile-icon">';
        } else {
            echo '<img src="icon/group-icon.png" alt="side icon" class="profile-icon">';
        }
        echo '<span class="in-profile-text">' . $row_group['groupName'] . '</span>';
        echo "</a>";
    }
}

?>

            </div>

        </aside>
        

<script>
const fbutton = document.getElementById('fbutton'); 
const gbutton = document.getElementById('gbutton'); 
const flist = document.getElementById('friend-container'); 
const glist = document.getElementById("group-container"); 


fbutton.addEventListener('click', () => {

     flist.style.display='block';
     glist.style.display='none';

});

gbutton.addEventListener('click', () => {

flist.style.display='none';
glist.style.display='block';

});
</script>
</html>

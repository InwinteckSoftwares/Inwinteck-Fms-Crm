document.getElementById('calculateTotal').addEventListener('click', function () {
    // Get the score values
    var greetFEScore = parseFloat(document.getElementById('GreetFEScore').value) || 0;
    var gdNameAndHandoverScore = parseFloat(document.getElementById('GDNameAndHandoverScore').value) || 0;
    var pregameManualSiteContactScore = parseFloat(document.getElementById('PregameManualSiteContactScore').value) || 0;
    var greetTSEScore = parseFloat(document.getElementById('GreetTSEScore').value) || 0;
    var commVerifSerialNumberScore = parseFloat(document.getElementById('CommVerifSerialNumberScore').value) || 0;
    var feFollowingInstructionScore = parseFloat(document.getElementById('FEFollowingInstructionScore').value) || 0;
    var partsDetailsScore = parseFloat(document.getElementById('PartsDetailsScore').value) || 0;
    var closerFormScore = parseFloat(document.getElementById('CloserFormScore').value) || 0;
    var ticketCreationScore = parseFloat(document.getElementById('TicketCreationScore').value) || 0;
    var thankYouScore = parseFloat(document.getElementById('ThankYouScore').value) || 0;

    // Validate the score values
    if (greetFEScore > 5) {
        alert("Greet FE Score cannot be more than 5.");
        return;
    }
    if (gdNameAndHandoverScore > 5) {
        alert("GD Name And Handover Score cannot be more than 5.");
        return;
    }
    if (pregameManualSiteContactScore > 5) {
        alert("Pregame Manual Site Contact Score cannot be more than 5.");
        return;
    }
    if (greetTSEScore > 5) {
        alert("Greet TSE Score cannot be more than 5.");
        return;
    }
    if (commVerifSerialNumberScore > 25) {
        alert("Comm & Verif Serial Number Score cannot be more than 25.");
        return;
    }
    if (feFollowingInstructionScore > 10) {
        alert("FE Following Instruction Score cannot be more than 10.");
        return;
    }
    if (partsDetailsScore > 10) {
        alert("Parts Details Score cannot be more than 10.");
        return;
    }
    if (closerFormScore > 15) {
        alert("Closer Form Score cannot be more than 15.");
        return;
    }
    if (ticketCreationScore > 15) {
        alert("Ticket Creation Score cannot be more than 15.");
        return;
    }
    if (thankYouScore > 5) {
        alert("Thank You Score cannot be more than 5.");
        return;
    }

    // Calculate the total score
    var totalScore = greetFEScore + gdNameAndHandoverScore + pregameManualSiteContactScore + greetTSEScore +
        commVerifSerialNumberScore + feFollowingInstructionScore + partsDetailsScore + closerFormScore +
        ticketCreationScore + thankYouScore;

    // Display the total score in the totalScore textarea
    document.getElementById('totalScore').value = totalScore;

});
//============================Show Score field based on User who has handeld ticket===========================//
document.getElementById('Handler1Name').addEventListener('change', function () {

    var scoreField = document.getElementById('Handler1Score');
    var remarkField = document.getElementById('Remark1');
    var remarkLabel = document.getElementById('remark1label');
    var remarkRow = document.getElementById('remark1Row');
    if (this.value) {
        scoreField.style.display = 'block';
        remarkField.style.display = 'block';
        remarkLabel.style.display = 'block';
        remarkRow.style.display = 'flex';

    } else {
        scoreField.style.display = 'none';
        remarkField.style.display = 'none';
        remarkLabel.style.display = 'none';
        remarkRow.style.display = 'none';
    }
});



document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('addHandledBy').addEventListener('click', function () {
        var container = document.getElementById('handledByContainer');
        var index = container.querySelectorAll('.form-row').length / 2 + 1; // Adjusted to count correctly

        if (index > 4) { // Limiting to a maximum of 4 handlers
            alert("You can't add more than 4 handlers");
            return;
        }

        var newFieldset = document.createElement('div');
        newFieldset.className = 'form-row';
        newFieldset.innerHTML = `
            <label for="Handler${index}Name" class="col-sm-2 col-form-label">Handled By ${index}</label>
            <div class="col-sm-5">
                <select class="form-control handledBySelect" name="Handler${index}Name" id="Handler${index}Name" required>
                    <option value="">Select</option>
                    ${Array.from(document.getElementById('Handler1Name').options).map(option => `<option value="${option.value}">${option.text}</option>`).join('')}
                </select>
            </div>
            <div class="col-sm-5">
                <input type="number" step="any" class="form-control" name="Handler${index}Score" id="Handler${index}Score" placeholder="Enter score" min="-100" max="100" style="display: none;">
            </div>
        `;
        container.appendChild(newFieldset);

        var newRemarkFieldset = document.createElement('div');
        newRemarkFieldset.className = 'form-row';
        newRemarkFieldset.innerHTML = `
            <label for="Remark${index}" class="col-sm-2 col-form-label">Remark ${index}</label>
            <div class="col-sm-10">
                <textarea class="form-control" name="Remark${index}" id="Remark${index}" placeholder="Enter remark" style="display: none;"></textarea>
            </div>
        `;
        container.appendChild(newRemarkFieldset);

        var newSelect = newFieldset.querySelector('select');
        newSelect.addEventListener('change', function () {
            var scoreField = document.getElementById(`Handler${index}Score`);
            var remarkField = document.getElementById(`Remark${index}`);
            if (this.value) {
                scoreField.style.display = 'block';
                remarkField.style.display = 'block';
            } else {
                scoreField.style.display = 'none';
                remarkField.style.display = 'none';
            }
        });
    });
});
//----------Disable update button when score is once updated-----------------------//

document.addEventListener('DOMContentLoaded', function () {
    var scoredByInput = document.getElementById('scoredBy');
    var updateButton = document.getElementById('updateButton');

    function checkConditions() {

        if (scoredByInput.value !== "") {
            updateButton.disabled = true;
        } else {
            updateButton.disabled = false;
        }
    }

    // Call checkConditions on page load
    checkConditions();
});


//===============Load FE details==========================//
function updateFEInfo(feId, prefix) {
    if (feId > 0) {
        $.get(baseurl + "Transaction/getFEInfo", { id: feId }, function (data) {
            if (data.res === "Success") {
                $('#' + prefix + '_ID').empty().append('<option value =' + data.CC['FE_ID'] + '>' + data.CC['FE_Name'] + '</option>');
                $('#' + prefix + '_contact').val(data.CC['Phone']);
                $('#' + prefix + '_Email').val(data.CC['Email']);
                $('#' + prefix + '_Origin').val(data.CC['FE_Origin']);
                $('#' + prefix + '_Certification').val(data.CC.Certifications);
            }
        });
    }
}
const firstFEId = $('#firstFEId').val();
const secondFEId = $('#secondFEId').val();

updateFEInfo(firstFEId, 'FE');
updateFEInfo(secondFEId, 'FE_2');

document.addEventListener('DOMContentLoaded', function () {
    var isEnabled = document.getElementById('enableSecondFE').checked;
    var secondFeChargesContainer = document.getElementById('secondChargesSection');
    if (isEnabled) {
        secondFeChargesContainer.style.display = 'block';
    }

});


//----------Time Zone Conversion Client Side ----------------------//
async function getLocalTime(lat, lon, inputTimeIST) {
    const apiKey = "AIzaSyBvhQbrULBXqk9wPb9RjTeacVk3A3g-ci8";
    const timestamp = Math.floor(new Date(inputTimeIST).getTime() / 1000); // Convert input IST time to timestamp in seconds
    const apiUrl = `https://maps.googleapis.com/maps/api/timezone/json?location=${lat},${lon}&timestamp=${timestamp}&key=${apiKey}`;

    try {
        const response = await fetch(apiUrl);
        const data = await response.json();

        if (data.status === "OK") {
            const dstOffset = data.dstOffset; // Daylight savings offset in seconds
            const rawOffset = data.rawOffset; // Time zone offset in seconds
            const totalOffset = dstOffset + rawOffset; // Total offset from UTC in seconds

            // Convert the input IST time to UTC
            const istTime = new Date(inputTimeIST);
            const utcTime = new Date(istTime.getTime() - (5.5 * 60 * 60 * 1000));

            const localTime = new Date(utcTime.getTime() + (totalOffset * 1000));

            const year = localTime.getFullYear();
            const month = (localTime.getMonth() + 1).toString().padStart(2, '0'); // Add leading zero if needed
            const day = localTime.getDate().toString().padStart(2, '0'); // Add leading zero if needed
            const hours = localTime.getHours().toString().padStart(2, '0'); // 24-hour format, add leading zero
            const minutes = localTime.getMinutes().toString().padStart(2, '0'); // Add leading zero if needed

            // Return the manually formatted date
            return `${year}-${month}-${day}T${hours}:${minutes}`;
        } else {
            console.error("Error fetching timezone data:", data.errorMessage);
            return null;
        }
    } catch (error) {
        console.error("Error during API call:", error);
        return null;
    }
}

function getESTTime(inputTimeIST) {
    const estTime = new Date(new Date(inputTimeIST).getTime() - (9.5 * 60 * 60 * 1000));

    const year = estTime.getFullYear();
    const month = (estTime.getMonth() + 1).toString().padStart(2, '0'); // Add leading zero if needed
    const day = estTime.getDate().toString().padStart(2, '0'); // Add leading zero if needed
    const hours = estTime.getHours().toString().padStart(2, '0'); // 24-hour format, add leading zero
    const minutes = estTime.getMinutes().toString().padStart(2, '0'); // Add leading zero if needed

    return `${year}-${month}-${day}T${hours}:${minutes}`;
}

async function updateTimes(istInputId, localInputId, estInputId) {
    const inTimeIST = document.getElementById(istInputId).value;

    if (inTimeIST) {
        const estTimeFormatted = getESTTime(inTimeIST);
        document.getElementById(estInputId).value = estTimeFormatted;

        const lat = document.getElementById('latitude').value;
        const lon = document.getElementById('longitude').value;

        const localTime = await getLocalTime(lat, lon, inTimeIST);
        if (localTime) {
            document.getElementById(localInputId).value = localTime; // Set the local time in the correct format
        }
    }
}

function attachBlurEvents() {
    const inTime2Element = document.getElementById('In_Time2');
    const outTime2Element = document.getElementById('Out_Time2');

    document.getElementById('In_Time').addEventListener('blur', () => updateTimes('In_Time', 'inTimeLocal', 'inTimeUs'));
    document.getElementById('Out_Time').addEventListener('blur', () => updateTimes('Out_Time', 'OutTimeLocal', 'OutTimeUs'));
    if (inTime2Element) {
        inTime2Element.addEventListener('blur', () => updateTimes('In_Time2', 'inTimeLocal2', 'inTimeUs2'));
    }

    if (outTime2Element) {
        outTime2Element.addEventListener('blur', () => updateTimes('Out_Time2', 'OutTimeLocal2', 'OutTimeUs2'));
    }

}

document.addEventListener('DOMContentLoaded', attachBlurEvents);

//--------Total Resolution Time-------------//
function calculateTotalResolutionTime() {
    // Calculate for primary FE
    var inTime = new Date(document.getElementById('In_Time').value);
    var outTime = new Date(document.getElementById('Out_Time').value);

    if (!isNaN(inTime) && !isNaN(outTime)) {
        var diffMs = outTime - inTime; // Difference in milliseconds
        var diffMins = Math.floor((diffMs / 1000) / 60); // Convert milliseconds to minutes

        var hours = Math.floor(diffMins / 60);
        var minutes = diffMins % 60;

        document.getElementById('totalResolutionTimeLocal').value = hours + " hours " + minutes + " minutes";
    } else {
        document.getElementById('totalResolutionTimeLocal').value = "Invalid time";
    }

    // Calculate for 2nd FE if enabled
    var inTime2Element = document.getElementById('In_Time2');
    var outTime2Element = document.getElementById('Out_Time2');

    if (inTime2Element && outTime2Element) {
        var inTime2 = new Date(inTime2Element.value);
        var outTime2 = new Date(outTime2Element.value);

        if (!isNaN(inTime2) && !isNaN(outTime2)) {
            var diffMs2 = outTime2 - inTime2; // Difference in milliseconds
            var diffMins2 = Math.floor((diffMs2 / 1000) / 60); // Convert milliseconds to minutes

            var hours2 = Math.floor(diffMins2 / 60);
            var minutes2 = diffMins2 % 60;

            document.getElementById('totalResolutionTimeLocal2').value = hours2 + " hours " + minutes2 + " minutes";
        }
    }
}

document.addEventListener("DOMContentLoaded", function () {
    // Primary FE
    document.getElementById('In_Time').addEventListener('change', calculateTotalResolutionTime);
    document.getElementById('Out_Time').addEventListener('change', calculateTotalResolutionTime);

    // Check if the 2nd FE elements are available before attaching event listeners
    var inTime2Element = document.getElementById('In_Time2');
    var outTime2Element = document.getElementById('Out_Time2');

    if (inTime2Element && outTime2Element) {
        inTime2Element.addEventListener('change', calculateTotalResolutionTime);
        outTime2Element.addEventListener('change', calculateTotalResolutionTime);
    }
});

function updateHiddenField() {
    var caseNo = document.getElementById("Case_No").value;
    console.log("method called", caseNo);   
    document.getElementById("Hidden_Case_No").value = caseNo;
    console.log(document.getElementById("Hidden_Case_No"));
    console.log(document.getElementById("Hidden_Case_No").value, "hidden field value");
}

document.addEventListener("DOMContentLoaded", function () {
    updateHiddenField();
});

var venderRemark = document.getElementById("venderRemark");
var qualityRemark = document.getElementById("qualityRemark");

if (venderRemark && venderRemark.value.trim() !== "") {
    venderRemark.readOnly = true;
    venderRemark.style.display = 'block'; // Ensures it is visible if needed
}

if (qualityRemark && qualityRemark.value.trim() !== "") {
    qualityRemark.readOnly = true;
    qualityRemark.style.display = 'block'; // Ensures it is visible if needed
}


//=================Quality hot Requiremnt========================//

document.getElementById('Sourcing').addEventListener('blur', calculateTotal);
document.getElementById('CertificationValue').addEventListener('blur', calculateTotal);
document.getElementById('Charges').addEventListener('blur', calculateTotal);
document.getElementById('MeetingSLA').addEventListener('blur', calculateTotal);




function calculateTotal() {
    var sourcing = parseFloat(document.getElementById('Sourcing').value) || 0;
    var certification = parseFloat(document.getElementById('CertificationValue').value) || 0;
    var charges = parseFloat(document.getElementById('Charges').value) || 0;
    var meetingSLA = parseFloat(document.getElementById('MeetingSLA').value) || 0;

    var total = sourcing + certification + charges + meetingSLA;
    document.getElementById('TotalHotRequirement').value = total.toFixed(2);
}

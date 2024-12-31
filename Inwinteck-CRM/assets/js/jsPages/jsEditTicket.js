/// <reference path="../moment.min.js" />

//===================EU status about FE =================================//
document.addEventListener('DOMContentLoaded', function () {
    var statusId = document.getElementById("PreviousStatusId");
    var statusIdnew = document.getElementById("Status");

    var ticketStatusHub = $.connection.sourceStatusAboutFE;
    ticketStatusHub.client.updateStatus = function (ticketEuSelection) {
        var currentTicketId = $('#ticketId').val();

        // Update the modal with the new status
        $('#status').text(ticketEuSelection.status || 'N/A');
        $('#feName').text(ticketEuSelection.FE_Name || 'N/A');
        $('#Remark').text(ticketEuSelection.Remark || 'N/A');
        $('#createdOn').text(ticketEuSelection.CreatedOn || 'N/A');
        $('#modifiedOn').text(ticketEuSelection.ModifiedOn || 'N/A');

        $('#euStatusModal').modal('show');
      //  }
    };
    $.connection.hub.start().done(function () {
        console.log("Connected to SourceStatusAboutFE Hub");
    });

    function getEUStatusAboutFE(showModal = false) {
        var ticketId = $('#ticketId').val();

        $.ajax({
            url: '/Transaction/GetEUStatusAboutFE',  // Adjust URL if needed
            type: 'GET',
            data: { ticketId: ticketId },
            success: function (data) {
                if (data.success !== false) {
                    if (data) {
                        //if (data.Status === "Accepted" && statusId.value == 1414) {
                        //    statusIdnew.value = 1372;
                        //    $(".sc").css('display', 'block');
                        //}

                        var createdOn = moment(parseInt(data.CreatedOn.replace(/[^0-9]/g, ''))).format('DD MMM YYYY [at] hh:mm A');
                        var modifiedOn = data.ModifiedOn ? moment(parseInt(data.ModifiedOn.replace(/[^0-9]/g, ''))).format('DD MMM YYYY [at] hh:mm A') : 'N/A';


                        // Update the modal with the new status
                        $('#status').text(data.Status || 'N/A');
                        $('#feName').text(data.FE_Name || 'N/A');
                        $('#Remark').text(data.Remark || 'N/A');
                        $('#createdOn').text(createdOn || 'N/A');
                        $('#modifiedOn').text(modifiedOn || 'N/A');

                        // Optionally show modal if showModal is true
                        if (showModal) {
                            $('#euStatusModal').modal('show');
                        }
                    }
                } else {
                    console.log('Failed to fetch status');
                }
            },
            error: function () {
                console.log('Error fetching status');
            }
        });
    }

    // Event listener for the button click to manually fetch the status
    $('#EUStatus').on('click', function () {
        getEUStatusAboutFE(true); // Pass true to show the modal on click
    });
});

//------------------Second FE Options--------------------------------------------//
document.addEventListener('DOMContentLoaded', function () {

    function toggleSecondFE() {
        var isSecondFEEnabled = document.querySelector('input[name="IsSecondFEEnabled"]:checked').value === 'true';
        var secondFEContainer = document.getElementById('secondFEContainer');
        secondFEContainer.style.display = isSecondFEEnabled ? 'block' : 'none';  // IsSecondFEEnable is true then block display
    }

    toggleSecondFE();

    var radioButtons = document.querySelectorAll('input[name="IsSecondFEEnabled"]');
    radioButtons.forEach(function (button) {
        button.addEventListener('change', toggleSecondFE);
    });
});

//--------------------------FE Details on Click---------------------------------//

function getFEInfo(id, feNumber) {
    var isSecondFEEnabled = document.querySelector('input[name="IsSecondFEEnabled"]:checked').value === 'true';

    console.table([feNumber, id, isSecondFEEnabled]);
    $.get(baseurl + "Transaction/getFEInfo", { id: id }, function (data) {
        console.log("data",data);
        if (data.res == "Success") {
            if (feNumber === 1) {
                $('#FE_ID').empty();
                $('#FE_ID').append('<option value =' + data.CC.FE_ID + '>' + data.CC.FE_Name + '</option>');
                $('#FE_contact').val(data.CC.Phone);
                $('#FE_Email').val(data.CC.Email);
                $('#FE_Origin').val(data.CC.FE_Origin);
                $('#FE_Certification').val(data.CC.Certifications);
            } else if (feNumber === 2 && isSecondFEEnabled) {
                // Check if the second FE section is enabled and if feNumber is 2
                $('#FE_2_ID').empty();
                $('#FE_2_ID').append('<option value =' + data.CC.FE_ID + '>' + data.CC.FE_Name + '</option>');
                $('#FE_2_contact').val(data.CC.Phone);
                $('#FE_2_Email').val(data.CC.Email);
                $('#FE_2_Origin').val(data.CC.FE_Origin);
                $('#FE_2_Certification').val(data.CC.Certifications);
            }
        } else {
            clearFEFields(feNumber);
            $('#FEerror').text('No FE Found in Master.');
            $('#FEmyModal').modal('show');
        }
    });
}
//======================If FE is Already Selected Show it's data==============================//
function updateFEInfo(feId, prefix) {
    if (feId > 0) {
        $.get("/Transaction/getFEInfo", { id: feId }, function (data) {
            console.log("FE Info Function");

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

//-----------------FE Charges Field----------------------------//

// Function to update the visibility of the second charges section based on radio button selection
function updateSecondChargesVisibility() {
    var secondChargesSection = document.getElementById('secondChargesSection');
    var inTime2ndFE = document.getElementById('inTime2ndFE');
    var outTime2ndFE = document.getElementById('outTime2ndFE');
    var totalResolutionTimeLocal2 = document.getElementById('totalResolutionTimeLocal2');
    var isEnabled = document.getElementById('enableSecondFE').checked;
    var statusId = document.getElementById('Status').value;

    // Toggle visibility of the second charges section
    secondChargesSection.style.display = isEnabled ? 'block' : 'none';

    // Show inTime2ndFE when enabled and status is 1415
    if (inTime2ndFE) {
        inTime2ndFE.style.display = (isEnabled && statusId == 1415) ? 'block' : 'none';
    }

    // Show both inTime2ndFE and outTime2ndFE when enab led and status is 20
    if (isEnabled && statusId == 20) {
        inTime2ndFE.style.display = 'block';
        outTime2ndFE.style.display = 'block';
        totalResolutionTimeLocal2.style.display = 'block';
    } else {
        outTime2ndFE.style.display = 'none';
    }
}

document.addEventListener('DOMContentLoaded', function () {
    // Attach change event to relevant elements
    ['enableSecondFE', 'disableSecondFE', 'Status'].forEach(function (id) {
        document.getElementById(id).addEventListener('change', updateSecondChargesVisibility);
    });

    // Initial check on page load
    updateSecondChargesVisibility();

    
    document.addEventListener('DOMContentLoaded', function () {
        // Initialize select2 for currency dropdown
        $('#Currency').select2({
            placeholder: 'Select Currency',
            allowClear: true,
            theme: 'bootstrap4'
        });
        // Function to update visibility of fields dynamically based on values
        function updateVisibility() {
            var secondSectionFields = ['#TravelCharge2', '#TravelAmount2', '#ChargeType2', '#PerJob2', '#PerHour2'];
            var shouldShowSecondSection = secondSectionFields.some(function (selector) {
                return $(selector).val();
            });

            // Show second section if necessary and disable "+" button
            if (shouldShowSecondSection) {
                $('.charges-section').eq(1).show();
                $('.add-section').prop('disabled', true);
            }

            // TravelCharge field visibility
            $('#travelAmountRow').toggle($('#TravelCharge').val() === 'True');
            $('#travelAmountRow2').toggle($('#TravelCharge2').val() === 'True');

            // ChargeType field visibility for both ChargeType and ChargeType2
            $('#perJobRow').toggle($('#ChargeType').val() === 'Per Job');
            $('#perHourRow').toggle($('#ChargeType').val() === 'Per Hour');
            $('#perJobRow2').toggle($('#ChargeType2').val() === 'Per Job');
            $('#perHourRow2').toggle($('#ChargeType2').val() === 'Per Hour');
        }

        // Bind change events for dynamic visibility
        ['TravelCharge', 'TravelCharge2', 'ChargeType', 'ChargeType2'].forEach(function (id) {
            $('#' + id).on('change', updateVisibility);
        });


        // Initial visibility check on page load
        updateVisibility();
    });
});


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

//------------Show Update button when Dispatched Dt is not available---------------//
document.addEventListener("DOMContentLoaded", function () {
    var dispatchDateLocalInput = document.querySelector('input[name="Dispatch_Date"]');
    var updateButtonContainer = document.getElementById("updateButtonContainer");

    // Check if Dispatch_Date (Local) has no value
    if (!dispatchDateLocalInput.value) {
        updateButtonContainer.style.display = "block";
        dispatchDateLocalInput.removeAttribute("readonly");
    }

    document.getElementById("updateDispatchDateBtn").addEventListener("click", function () {
        var ticketId = document.getElementById("ticketId").value;
        var caseDate = document.getElementById("CaseDtLocal").value;
        var dispatchedDate = dispatchDateLocalInput.value;
        var lat = document.getElementById("latitude").value;
        var lng = document.getElementById("longitude").value;

        var data = {
            ticketId: ticketId,
            caseDate: caseDate,
            dispatchedDate: dispatchedDate,
            lat: lat,
            lng: lng
        };
        $.ajax({
            url: '/Transaction/UpdateDispatchDateBtn',
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Include the token here
            },
            success: function (response) {
                window.location.href = '/Transaction/editTicket/' + ticketId;
            }
        });

    });
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

//----------------To Generate Closer Form--------------------------//

document.addEventListener('DOMContentLoaded', () => {
    const generateButton = document.getElementById('generateBtn');

    generateButton.addEventListener('click', () => {
        // Get field values
        const onsiteDate = document.getElementById('Dispatch_Date').value;
        const oemName = document.getElementById('OEM').selectedOptions[0]?.text || '';
        const sourceCase = document.getElementById('Case_No').value;
        const oemCase = document.getElementById('Tracking_number').value;
        const feName1 = document.getElementById('FE_ID').selectedOptions[0]?.text || '';
        const feName2 = document.getElementById('FE_2_ID').selectedOptions[0]?.text || 'N/A';
        const checkInTime = document.getElementById('inTimeLocal').value;
        const checkOutTime = document.getElementById('OutTimeLocal').value;
        const totalResolutionTime = calculateResolutionTime(checkInTime, checkOutTime);
        const actionsPerformed = document.getElementsByName('Job_Description')[0]?.value || '';
        const tseName1 = document.getElementById('TSE_Name').value;
        const tseName2 = document.getElementById('TSE_Name2')?.value || 'N/A';
        const tseName3 = document.getElementById('TSE_Name3')?.value || 'N/A';

        // Retrieve distinct "Modified By" names and "Created By"
        const createdBy = document.getElementById('th_CreatedBy').value || ''; // Get "Created By" value
        const distinctISENames = getDistinctISENames(createdBy); // New function to get all distinct ISE names
        const ticketNo = document.getElementsByName('Ticket_No')[0]?.value || '';

        // Construct the closure form content
        const closureFormContent = `
        Closure Form
        --------------------------
        Onsite Date –  ${formatDate(onsiteDate)}
        OEM Name – ${oemName}
        Source Case # ${sourceCase}
        OEM Case # ${oemCase}
        S/N or repaired - 
        FE Name – ${feName1} ${feName2 !== 'N/A' ? 'and ' + feName2 : ''}
        Check-In Time – ${formatTime(checkInTime)}
        Check-Out Time – ${formatTime(checkOutTime)}
        Total Resolution Time – ${totalResolutionTime}
        Actions performed – ${actionsPerformed}
        TSE Name – ${tseName1}${tseName2 !== 'N/A' ? ', ' + tseName2 : ''}${tseName3 !== 'N/A' ? ', ' + tseName3 : ''}
        ISE Name – ${distinctISENames.join(', ')}
        Parts – 
        Ticket No – ${ticketNo}`;

        // Insert the generated content into the textarea
        document.getElementById('closureForm').value = closureFormContent;
    });

    // New function to get all distinct ISE names (Created By + Modified By)
    const getDistinctISENames = (createdBy) => {
        const iseNames = new Set(); // Use Set to ensure distinct names

        // Add "Created By" to the Set
        if (createdBy) {
            const firstName = extractFirstName(createdBy);
            iseNames.add(firstName);
        }

        // Add all "Modified By" names from the ticket history table to the Set
        const modifiedByElements = document.querySelectorAll('#Show tbody tr td:first-child');
        modifiedByElements.forEach(element => {
            const name = element.textContent.trim();
            const firstName = extractFirstName(name);
            iseNames.add(firstName); // Add first names to Set (automatically filters duplicates)
        });

        return Array.from(iseNames); // Convert Set to Array for easier manipulation
    };

    // Helper function to extract only the first name from an email or full name
    const extractFirstName = (name) => {
        const firstName = name.split('@')[0].split('.')[0]; // Extract part before the first dot
        return firstName.charAt(0).toUpperCase() + firstName.slice(1); // Capitalize the first letter
    };

    // Helper function to format the Onsite Date
    const formatDate = (dateString) => {
        if (!dateString) return '';
        const options = { day: '2-digit', month: 'long', year: 'numeric' };
        const date = new Date(dateString);
        return date.toLocaleDateString('en-GB', options); // Formats as "05 September 2024"
    };

    // Helper function to format Check-In and Check-Out times
    const formatTime = (timeString) => {
        if (!timeString) return '';
        const options = { hour: '2-digit', minute: '2-digit', hour12: true }; // Formats as "09:28 AM"
        const time = new Date(timeString);
        return time.toLocaleTimeString('en-US', options) + " Local Time";
    };

    // Helper function to calculate total resolution time
    const calculateResolutionTime = (startTime, endTime) => {
        const start = new Date(startTime);
        const end = new Date(endTime);
        const diffMs = end - start; // Difference in milliseconds
        const diffHrs = Math.floor((diffMs % 86400000) / 3600000); // Hours
        const diffMins = Math.round(((diffMs % 86400000) % 3600000) / 60000); // Minutes
        return `${diffHrs} hours, ${diffMins} minutes`;
    };
});
//----------------------To Generate Closer Form End-----------------------------//

//----------Condition on Out Time-----------------//
document.addEventListener('DOMContentLoaded', function () {
    // Function to compare date and time
    function validateTime(inTimeId, outTimeId, errorMessageId) {
        var inTime = document.getElementById(inTimeId).value;
        var outTime = document.getElementById(outTimeId).value;

        if (inTime && outTime) {
            var inDateTime = new Date(inTime);
            var outDateTime = new Date(outTime);

            if (outDateTime < inDateTime) {
                document.getElementById(errorMessageId).innerText = "Out Time cannot be earlier than In Time.";
            } else {
                document.getElementById(errorMessageId).innerText = "";
            }
        }
    }

    // Attach validation for In_Time and Out_Time
    document.getElementById('Out_Time').addEventListener('change', function () {
        validateTime('In_Time', 'Out_Time', 'outTimeError');
    });

    document.getElementById('In_Time').addEventListener('change', function () {
        validateTime('In_Time', 'Out_Time', 'outTimeError');
    });

    // Check if In_Time2 and Out_Time2 exist before applying the validation
    var outTime2Field = document.getElementById('Out_Time2');
    var inTime2Field = document.getElementById('In_Time2');

    if (outTime2Field && inTime2Field) {
        outTime2Field.addEventListener('change', function () {
            validateTime('In_Time2', 'Out_Time2', 'outTime2Error');
        });

        inTime2Field.addEventListener('change', function () {
            validateTime('In_Time2', 'Out_Time2', 'outTime2Error');
        });
    }
});


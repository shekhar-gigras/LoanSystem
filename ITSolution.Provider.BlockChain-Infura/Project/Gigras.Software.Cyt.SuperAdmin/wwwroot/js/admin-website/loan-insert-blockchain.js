async function setCommonAndInterestDetails(loanid,formData) {
    let data = [];
    data.loanId = loanid || ""; // Default to an empty string if loanid is null or undefined
    data.emiPaymentStartDate = formData.get("EMIPaymentDate") || ""; // Default to empty string
    data.principalAmount = formData.get("PrincipalAmount") || 0; // Default to 0
    data.fixedInterestRate = formData.get("InterestRate") || 0; // Default to 0
    data.monthlyPaymentAmount = formData.get("MonthlyPaymentAmount") || 0; // Default to 0
    data.maturityDate = formData.get("MaturityDate") || ""; // Default to empty string
    data.emiPaymentDay = formData.get("DayOfMonth") || 0; // Default to 0
    data.interestRateChangeDate = formData.get("ChangeInterestRateDate") || ""; // Default to empty string
    data.lenderName = formData.get("LenderName") || ""; // Default to empty string
    data.borrowerName = formData.get("BorrowerName") || ""; // Default to empty string

    return data;
}

async function setPaymentAndLateDetails(loanid, formData) {
    const data = [];
    data.loanId = loanid || ""; // Default to an empty string if loanid is null or undefined
    data.latePaymentGracePeriod = formData.get("LatePaymentGracePeriod") || 0; // Default to 0
    data.lateChargePercentage = formData.get("LateChargePercentage") || 0; // Default to 0
    data.margin = formData.get("Margin") || 0; // Default to 0
    data.currentIndex = formData.get("CurrentIndex") || 0; // Default to 0
    data.maxInterestRateAtFirstChange = formData.get("MaxInterestRateFirstChangeDate") || 0; // Default to 0
    data.minInterestRateAtFirstChange = formData.get("MinInterestRateFirstChangeDate") || 0; // Default to 0
    data.maxInterestRateAfterChange = formData.get("MaxSubsequentInterestRateAfterChangeDate") || 0; // Default to 0
    data.minInterestRateAfterChange = formData.get("MinSubsequentInterestRateAfterChangeDate") || 0; // Default to 0

    return data;
}
async function setAdjustableInterestRateDetails(loanid, formData) {
    const data = [];
    data.loanId = loanid || ""; // Default to an empty string if loanid is null or undefined
    data.noteDate = formData.get("NoteDate") || ""; // Default to an empty string
    data.city = formData.get("City") || ""; // Default to an empty string
    data.state = formData.get("State") || ""; // Default to an empty string
    data.propertyAddress = formData.get("PropertyAddress") || ""; // Default to an empty string
    data.paymentLocation = formData.get("PaymentLocation") || ""; // Default to an empty string

    return data;
}

async function getLoanDetails(loanid, formData) {
    // Execute all the functions concurrently and wait for the results
    const [commonAndInterestDetails, paymentAndLateDetails, adjustableInterestRateDetails] = await Promise.all([
        setCommonAndInterestDetails(loanid, formData),
        setPaymentAndLateDetails(loanid, formData),
        setAdjustableInterestRateDetails(loanid, formData)
    ]);

    // Merge the results into a single object
    const data = {
        ...commonAndInterestDetails,
        ...paymentAndLateDetails,
        ...adjustableInterestRateDetails
    };

    return data;
}
async function getLoanApiDetails(loanid) {
    try {
        const response = await fetch(`/api/loandetails/${loanid}`);
        if (!response.ok) {
            // If the response status is not OK, throw an error
            Swal.fire("Failed", `Failed to fetch loan details: ${response.statusText}`, "error");
        }


        const jsonData = await response.json(); // Parse the JSON response

        if (jsonData && Object.keys(jsonData).length > 0) {
            let loanData = {
                loanId: loanid || "", // Use the loanid provided in the function
                emiPaymentStartDate: jsonData.eMIPaymentDate || "", // Extract from jsonData
                principalAmount: jsonData.principalAmount || 0, // Extract from jsonData, default to 0
                fixedInterestRate: jsonData.interestRate || 0, // Extract from jsonData, default to 0
                monthlyPaymentAmount: jsonData.monthlyPaymentAmount || 0, // Extract from jsonData, default to 0
                maturityDate: jsonData.maturityDate || "", // Extract from jsonData
                emiPaymentDay: jsonData.dayOfMonth || 0, // Extract from jsonData, default to 0
                interestRateChangeDate: jsonData.changeInterestRateDate || "", // Extract from jsonData
                lenderName: jsonData.lenderName || "", // Extract from jsonData
                borrowerName: jsonData.borrowerName || "" // Extract from jsonData
            };
            return loanData;
        } else {
            Swal.fire("No Data", "Loan details are unavailable.", "info");
            return null;
        }
    } catch (error) {
        // Display error in SweetAlert2
        Swal.fire({
            title: "Error",
            text: error.message || "An error occurred while fetching loan details.",
            icon: "error",
            confirmButtonText: "OK"
        });
        return null;
    }
}

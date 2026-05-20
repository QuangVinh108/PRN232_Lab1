const fs = require('fs');
const pdf = require('pdf-parse');

let dataBuffer = fs.readFileSync('../PRN232- LAB 1 - Rest API Basics and Deployment.pdf');

pdf(dataBuffer).then(function(data) {
    fs.writeFileSync('../pdf_text.txt', data.text);
    console.log("Done extracting PDF.");
}).catch(function(error) {
    console.error("Error parsing PDF:", error);
});

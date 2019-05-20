// try to make file listing
const path = require('path');
const fs = require('fs');
const directoryPath = path.join(__dirname, 'Test_Documents');
// const http = require('http');
// const https = require('https');
const url = require('url');
var cors = require('cors');
const basicAuth = require('express-basic-auth');


var express = require('express');
var multer = require('multer');
var docxConverter = require('docx-pdf');
var app = express();
var port = 8080
var count = 0;

var storage = multer.diskStorage({
  destination: function(req, file, cb) {
    cb(null, directoryPath);
  },
  filename: function(req, file, cb) {
    cb(null, file.originalname + "_annots" + ".xml");
  }
});

//var upload = multer({ dest: directoryPath });
var upload = multer({storage: storage});
fs.writeFileSync('nuclearLog.txt', " ");

const gm = require('gm').subClass({imageMagick: true});



// Create JPG from page 0 of the PDF


app.use(cors());
// app.use(basicAuth({
//   users: {'admin' : 'admin'},
//   challenge:  true
// }));

app.use(express.static(directoryPath));


app.get('/getFiles', function(request, response) {
  var request = url.parse(request.url, true);
  var action = request.pathname;
  //console.log(request);
  var retarr = [];

    fs.readdir(directoryPath, function (err, files) {
    //handling error
    if (err) {
        return console.log('Unable to scan directory: ' + err);
    }
    //listing all files using forEach
    files.forEach(function (file) {
        // Do whatever you want to do with the file
        console.log(file);
        retarr.push(file);
        if(path.extname(file) == ".pdf"){
          gm(directoryPath +"/"+file+"[0]")
              .thumb(
                  200, // Width
                  200, // Height
                  directoryPath + "/" + path.basename(file, ".pdf") + '.png', // Output file name
                  80, // Quality from 0 to 100
                  function (error, stdout, stderr, command) {
                      if (!error) {
                          console.log(command);
                      } else {
                          console.log(error);
                      }
                  }
              );
        } else if(path.extname(file) == '.docx'){
            docxConverter(directoryPath+"/"+file, directoryPath+"/"+path.basename(file, ".docx")+".pdf",
                          function (error, stdout, stderr, command) {
                            if(!error){
                              gm(directoryPath +"/"+path.basename(file, ".docx") + ".pdf"+"[0]")
                                  .thumb(
                                      200, // Width
                                      200, // Height
                                      directoryPath + "/" + path.basename(path.basename(file, ".docx"), ".pdf") + '.png', // Output file name
                                      80, // Quality from 0 to 100
                                      function (error, stdout, stderr, command) {
                                          if (!error) {
                                              console.log(command);
                                              fs.unlink(directoryPath +"/"+path.basename(file, ".docx") + ".pdf", function(error, stdout, stderr, command){});

                                          } else {
                                              console.log(error);
                                          }
                                      }
                                  );
                            } else {

                            }
                          });
        }
    });
    //response.writeHead(200,{"Content-Type" : "application/json"});
    response.send(retarr);
    response.end();
  });
});

// app.get('/download/:filename', function(request, response){
//   //var request = url.parse(request.url, true);
//   var action = request.pathname;
//   var name = request.params.filename;
//   console.log(name);
//   response.download(directoryPath + "/" + name);
// });

app.post("/uploadFile", upload.single('fileToUpload'), function(request, response) {
    if(request.file) {
      response.end("Uploaded Successfully");
    } else {
      response.end("Error in upload");
    }
});

app.post("/logNuclear", function(request, response) {
  count++;
  fs.appendFileSync('nuclearLog.txt', "logged nuclear stamp, count at " + count + "\n");
  response.end("updated nuclear count");
});

app.get("/getNuclears", function(request, response){
  response.end(count.toString());
});

app.post("/saveAnnotations", upload.single('annotations'), function(request, response){
  response.end("saved to server");
  console.log(request.header);
});

app.listen(port);

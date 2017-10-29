To run server on VM:
1. copy all
2. run: [sudo] source ./openServer.sh

Dependencies:
run the following commands once under /webServer in order to install virtualenv:
$ [sudo] pip install virtualenv
$ mkdir ~/envs
$ virtualenv ~/envs/lsbaws/

you should install the python libraries:
numpy+mlk
cv2
urllib
json
sklearn.cluster
matplotlib
skimage

In order to create a skin-detect request:
curl <server-address>/?light=<light num>&fileName=<name of file on server>&imgUrl=<URL to load pic from>

In order to create a report-gen request:
curl <server-address> --data @"example.json" -H "Content-Type: application/json"
import matplotlib
matplotlib.use('Agg')
import json
import datetime
import numpy as np
from matplotlib.backends.backend_pdf import PdfPages
import matplotlib.pyplot as plt
import matplotlib.image as mpimg
import matplotlib.dates as dates
import smtplib
import time, datetime
from email.mime.text import MIMEText
from os.path import basename
from email.mime.application import MIMEApplication
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
import sys
from email.utils import COMMASPACE, formatdate
import random
import matplotlib.dates as mdates
import matplotlib.ticker as ticker
from matplotlib.ticker import MultipleLocator, FuncFormatter
import urllib2
from skimage import io

def getCradentialsFromConfig():
    with open("config.txt", "r") as fil:
        result = fil.readline().strip(), fil.readline().strip(), fil.readline().strip()
        fil.close()
        return result

def getRandomTipFromConfig():
    with open("config.txt", "r") as fil:
        for line in fil:
            if line.startswith("tips:"):
                tip_list = line[5:len(line)].strip().split(":::")
                result = tip_list[random.randint(0, len(tip_list) - 1)]
                fil.close()
                return result
        return "Sorry, but there was an error generating the tip"

def getSkinToneTextFromConfig(skinTone_num):
    with open("config.txt", "r") as fil:
        for line in fil:
            if line.startswith("Skin Tone Text for skin #{num}:".format(num = skinTone_num)):
                result = line[len("Skin Tone Text for skin #0:"):len(line)].strip()
                fil.close()
                return result
        return "Sorry, but there was an error generating the skin tone description"
    
def getMailTxtMsg():
    return "Hi!\nEnclosed your SunnyDay report.\nRegards,\nSunnyDay"

def send_mail(mail_to, subject, email_content, path_to_file, file_dn):
    sender, userName, password = getCradentialsFromConfig()
    receivers = mail_to
    smtpObj = smtplib.SMTP(host='smtp.gmail.com', port=587)
    smtpObj.ehlo()
    smtpObj.starttls()
    smtpObj.ehlo()

    msg = MIMEMultipart()
    msg['From'] = sender
    msg['To'] = receivers
    msg['Date'] = formatdate(localtime=True)
    msg['Subject'] = subject

    msg.attach(MIMEText(email_content))

    with open(path_to_file, "rb") as fil:
        part = MIMEApplication(fil.read(),Name=file_dn)
        part['Content-Disposition'] = 'attachment; filename="%s"' % file_dn
        msg.attach(part)
    smtpObj.login(userName, password)
    smtpObj.sendmail(sender, receivers, msg.as_string())
    smtpObj.quit()

def plotGraph(x, y, ax, graph_title="", added_txt="", x_lab="", y_lab=""):
    x_vec = np.array(x)
    y_vec = np.array(y)    
    ax.bar(x, y, width=0.8)
    ax.xaxis_date()
   
    #ax.set_xticklabels(x_lables,fontsize = 3)
    ax.set_title(graph_title)
    ax.set_ylabel(y_lab)
    #ax.set_xticklabels(ax.xaxis.get_majorticklabels(), rotation=45)
    ax.tick_params(labelsize=3)
    ax.set_xlabel(x_lab)
    ax.text(0, 0, added_txt, wrap='True', fontsize = 4, ha = 'left', va = 'bottom')

def format_hist(x, pos):
    if x < 12:
        return int(x)
    else:
        return ""
        
def plotHist (x, ax, graph_title="", added_txt="", x_lab="", y_lab=""):
    print (str(x))
    x_vec = np.array(x)
    #ax.hist(x, bins=[1 for i in range(1, 12)])
    strategy = [1,2,3,4,5,6,7,8,9,10,11]
    value = np.array(x)
    strategies = np.array(strategy)
    ax.bar(strategy, value, .8)
    ax.axis([1,12,0,max(x)])
    ax.set_title(graph_title)
    ax.set_xlabel(x_lab)
    ax.set_ylabel(y_lab)
    #ax.set_xticks([i - 0.5 for i in range(1, 12)])
    #ax.set_xticklabels(range(1, 12))
    ax.xaxis.set_major_locator(MultipleLocator(1))
    ax.xaxis.set_major_formatter(FuncFormatter(format_hist))
    ax.text(0, 0, added_txt, ha = 'left', va = 'bottom', wrap='True', fontsize = 4)

def getSkinToneTxt(skinTone_num):
    #TODO: add real txt
    if skinTone_num >= 7 or skinTone_num < 0:
        return "We don't have enough infrmation regarding your skin type. Please consider taking an updated picture using the mobile app\n"
    else:
        return "Your skin tone type is {skinTone} in the Fitzpatrick scale\n{SkinToneTxt}\n".format(skinTone = skinTone_num, SkinToneTxt = getSkinToneTextFromConfig(skinTone_num))

def getAlarmText(numOfAlarms):
    return "We regularly send alerts reminding you to put on sunscreen. We have sent you " + str(numOfAlarms) + " such alerts."

def getTip():
    tip = getRandomTipFromConfig()
    return "Tip: {tip}.\n".format(tip = tip)

def getSPFText(skin_num, spf_num):
    #TODO: add real txt
    if skin_num >= 7 or skin_num <= 0:
        return ""
    else:
        if skin_num < 4:
            recommended_SPF = 30
        else:
            recommended_SPF = 15
        if recommended_SPF < spf_num:
            return "You are not using the best sunscreen! We recommend you will use sunscreen with an SPF level of {recommended_SPF}+\n".format(recommended_SPF = recommended_SPF)
        else:
            return "You are using a suitable sunscreen. The minimal SPF recommended for your screen tone is {recommended_SPF}\n".format(recommended_SPF = recommended_SPF)

def getReadTimeList(UvReadings_list):
    result = []
    for read in UvReadings_list:
        result.append(read['ReadTime'])
    return result

def getReadUvReadList(UvReadings_list):
    result = []
    for read in UvReadings_list:
        result.append(read['UVRead'])
    resultsArr = [0 for i in range(0, 11)]
    for res in result:
        resultsArr[res] += 1
    return resultsArr

def getPlotData(UvPerDay):
    x = []
    y = []
    for read in UvPerDay:
        print str(read)
        x.append(datetime.datetime.utcfromtimestamp(read['ReadTime']))
        #x.append(read['ReadTime'])
        y.append(read['UVRead'])
    return x, y

def createReportFig(data):
    fig = plt.figure()
    hist_ax = fig.add_axes([0.1, 0.25, 0.35, 0.4])
    hist_txt_ax = fig.add_axes([0.1, 0.14, 0.35, 0.1])
    hist_txt_ax.axis('off')
    plot_ax = fig.add_axes([0.6, 0.25, 0.35, 0.4])
    plot_txt_ax = fig.add_axes([0.6, 0.14, 0.35, 0.1])
    plot_txt_ax.axis('off')
    welcome_ax = fig.add_axes([0.22, 0.75, 0.60, 0.1], frameon=True)
    welcome_ax.axis('off')
    pic_ax = fig.add_axes([0.1, 0.75, 0.1, 0.1], frameon=True)
    pic_ax.axis('off')
    tip_ax = fig.add_axes([0.1, 0.02, 0.9, 0.1], frameon=True)
    tip_ax.axis('off')

    welcometxt = "{name}'s SunnyDay Report - {date}\n\n".format(name = data['userName'], date = datetime.datetime.now().strftime("%B %d, %Y"))
    skinToneTxt = getSkinToneTxt(data['SkinTone'])
    spfTxt = getSPFText(data['SkinTone'], data['SpfLevel'])
    print str(data)
    alarmTxt = getAlarmText(len(data['Alarms']))
    welcome_ax.text(0, 0, (welcometxt+skinToneTxt+spfTxt+alarmTxt).strip(), wrap='True')
    #TODO: URL pic
    f = io.imread(data['ImageUrl'])
    #image = matplotlib.image.imread(f)
    pic_ax.imshow(f)
    #time_list = [time.strftime('%Y-%m-%d', time.localtime(sec)) for sec in sorted(getReadTimeList(data['UvPerDay']))]
    #plotGraph(sorted(getReadTimeList(data['UvPerDay'])),getReadUvReadList(data['UvPerDay']), time_list, plot_ax, graph_title="UV Exposore by Day", added_txt="We collect the number of minutes per day in which you are exposed to UV light.\nThe above graph displays the number of exposure minutes by day.", x_lab="Time", y_lab="uvRead")
    uv_hist = plotHist(getReadUvReadList(data['UvReadings']), hist_ax, graph_title="UV Read Histogram", x_lab="UV Read", y_lab="Freq")
    x_plot, y_plot = getPlotData(data['UvPerDay'])
    uv_plot = plotGraph(x_plot, y_plot, plot_ax, graph_title="UV Exposore Time by Day", x_lab="Day", y_lab="Minutes")
    fig.autofmt_xdate()
    tip = getTip()
    plot_txt_ax.text(0, 0,  "We collect the number of minutes per day in which you are\nexposed to UV light. The above graph displays the number of\nexposure minutes by day.",
                     fontsize=5)
    #plot_txt_ax.text(0, 0, "We collect the number of minutes per day in which you are exposed to UV light.\nThe above graph displays the number of exposure minutes by day.", fontsize=5)
    hist_txt_ax.text(0, 0, "We collect the amount of UV light you are exposed to. The above\ngraph displays the frequency of different UV levels exposure.", fontsize=5)
    tip_ax.text(0,0, tip, wrap = 'True')
    fig.autofmt_xdate()

def createReportChildFig(data):
    txt = ""
    for child in data["childUsers"]:
        txt += "{name}'s SunnyDay Report - {date}\n\n".format(name = child['userName'], date = datetime.datetime.now().strftime("%B %d, %Y"))
        txt += getSkinToneTxt(child['SkinTone'])
        txt += getSPFText(child['SkinTone'], child['SpfLevel'])
        txt += getAlarmText(len(child['Alarms']))
        txt += "\n\n"
    fig = plt.figure()
    main_ax = fig.add_axes([0.1, 0.9, 0.8, 0.8], frameon=True)
    main_ax.axis('off')
    main_ax.text(0, 0, txt,ha = 'left', va = 'top', wrap = 'True') # 

def main(argv):
    #argv = ["example.json"] #This is for testing only!
    if (len(argv) != 1):
        print "This program should get 1 command line arg: path to JSON file"
        return
    json_file = argv[0] # "example.json"
    print "got JSON"
    with open(json_file) as data_file:
        data = json.load(data_file)
    receiver = data['email']
    print "email: " + receiver
    plt.rcParams.update({'font.size': 8})
    pp = PdfPages('report.pdf')
    parent_fig = createReportFig(data)
    pp.savefig(parent_fig)
    pp.savefig(createReportChildFig(data))       
    pp.close()
    print "created report"
    send_mail(receiver, "SunnyDay Report", getMailTxtMsg(), "report.pdf", "report.pdf")
    print "sent email"

if __name__ == "__main__":
    main(sys.argv[1:])

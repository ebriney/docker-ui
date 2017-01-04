import Cocoa
import WebKit

class ViewController: NSViewController, WKScriptMessageHandler {

    var webView: WKWebView!
    let clientExecutable = URL(fileURLWithPath: Bundle.main.path(forResource: "docker-client", ofType: nil)!).absoluteString.replacingOccurrences(of: "file://", with: "")
    
    override func loadView() {
        super.loadView()
        
}
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        let url = URL(fileURLWithPath: Bundle.main.path(forResource: "index", ofType: "html")!)
        let request = URLRequest(url: url)

        let contentController = WKUserContentController();
        let userScript = WKUserScript(source: "redHeader()", injectionTime: WKUserScriptInjectionTime.atDocumentEnd, forMainFrameOnly: true)
        contentController.addUserScript(userScript)
        contentController.add(self, name: "info")
        
        let config = WKWebViewConfiguration()
        config.userContentController = contentController
        
        self.webView = WKWebView(frame: self.view.frame, configuration: config)
        self.view = self.webView!
        webView.load(request)
    }

    override var representedObject: Any? {
        didSet {
        // Update the view, if already loaded.
        }
    }

    // WKScriptMessageHandler protocol
    func userContentController(_ userContentController: WKUserContentController, didReceive message: WKScriptMessage) {
        if(message.name == "info") {
            let info = getDockerInfo().replacingOccurrences(of: "\n", with: "")
            let js = "dockerInfoChanged( JSON.parse(\'\(info)\') );"
            self.webView.evaluateJavaScript(js) { (_, error) in
                print(error ?? "Info updated successfully")
            }
        } }

    // Execute a shell command and return (error?, stdout)
    func Execute(_ command: String) -> (String?, String) {
        let task = Process()
        task.launchPath = "/bin/sh"
        task.arguments = ["-c", command]
        let standardOutput = Pipe()
        let standardError = Pipe()
        task.standardOutput = standardOutput
        task.standardError = standardError
        task.launch()
        task.waitUntilExit()
        let output = String(data: standardOutput.fileHandleForReading.availableData, encoding: String.Encoding.utf8) ?? ""
        if task.terminationReason == Process.TerminationReason.exit && task.terminationStatus == 0 {
            return (nil, output)
        }
        return (String(data: standardError.fileHandleForReading.availableData, encoding: String.Encoding.utf8) ?? "" , output)
    }
    
    func getDockerInfo() -> String {
        let (error, output) = Execute(clientExecutable + " -info -compact")
        if let error = error {
            return error
        }
        return output
    }

}


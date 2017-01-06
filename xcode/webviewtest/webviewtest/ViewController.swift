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
        contentController.add(self, name: "docker")
        
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
        if(message.name == "docker") {
            let command = message.body as! String
            let output = dockerClient(command)
            let firstWord = command.components(separatedBy: " ").first!
            let js = "docker_\(firstWord)( JSON.parse(\'\(output)\') );"
            self.webView.evaluateJavaScript(js) { (_, error) in
                print(error ?? "docker-client \(command) succeeded")
            }
        }
    }

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
    
    func dockerClient(_ command: String) -> String {
        let (error, output) = Execute("\(clientExecutable) -compact -\(command)")
        if let error = error {
            return error
        }
        return output.replacingOccurrences(of: "\n", with: "")
    }

}


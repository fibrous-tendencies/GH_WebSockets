using HTTP

WebSockets.listen("127.0.0.1", 2000) do ws
    for msg in ws
        println(msg)
        # simple echo server
        WebSockets.send(ws, msg)
    end
end

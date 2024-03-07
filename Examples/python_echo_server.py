import websockets
import asyncio

async def handler(websocket):
    while True:
        message = await websocket.recv()
        try:
            print(message)

            await websocket.send(message)
        
        except Exception as e:
            print(e)
            print("Error in message")
            continue
        
async def main():
    async with websockets.serve(handler, "127.0.0.1", 2000) as ws:
        await asyncio.Future() # run forever


asyncio.run(main())

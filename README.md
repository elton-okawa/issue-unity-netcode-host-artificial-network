# Issue Host artificial network conditions

**Expected Behavior**: both `host` and `clients` being affected by artificial network conditions

**Actual Behavior**: only `clients` are affected

<img src="https://user-images.githubusercontent.com/24387035/186885475-cec37d30-a6ff-4ef0-923f-a70fbfa908df.png" width="425"/> <img src="https://user-images.githubusercontent.com/24387035/186885583-a52a9dc8-f353-466c-9d37-1c7e46c238cc.png" width="425"/>

## Approach
Note that both approaches differ in `step 2 and 3`

Network Var Ping (each client has a instance for it)
1. `client` send `rpc` with time `t1 = current time`
2. `server` changes `Network Var` with received time `t1`
3. `client` receives the `Network Var` update with `t1`
4. `client` stores the difference `currentTime - t1`
5. `client` calculates mean RTT
6. `client` updates text
7. Repeat `1 to 6`

Rpc Ping
1. `client` send `rpc` with time `t1 = current time`
2. `server` reply `rpc` with received time `t1`
3. `client` receives the `rpc` with `t1`
4. `client` stores the difference `currentTime - t1`
5. `client` calculates mean RTT
6. `client` updates text
7. Repeat `1 to 6`

## Step to reproduce

1. Clone this repository
2. Find `NetworkManager` object on scene, go to component `Unity Transport -> Debug Simulator`. Note that `delay` is set to `100ms`
3. Open `ParrelSync -> Clones Manager`
4. Click `Add new clone`
5. After a while, there'll be a option called `Open in new Editor` click on it
6. In this new editor start the game and select `Host`
7. In the previous editor also start the game and select `Client`
8. View both `Host` and `Client` pings

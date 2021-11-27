# Approach
Note that both approaches differ in `step 2 and 3`

Network Var Ping
1. `client` send `rpc` with time `t1 = current time`
2. `server` changes `Network Var` with received time `t1`
3. `client` receives the `Network Var` update with `t1`
4. `client` stores the difference `currentTime - t1`
5. `client` calculates mean RTT
6. `client` updates text
7. Repeat `1 to 6`

Rpc Ping
1. `client` send `rpc` with time `t1 = current time`
2. `server` send `rpc` with received time `t1`
3. `client` receives the `rpc` with `t1`
4. `client` stores the difference `currentTime - t1`
5. `client` calculates mean RTT
6. `client` updates text
7. Repeat `1 to 6`

# Step to reproduce

1. Clone this repository
2. Open `Netcode -> Simulator Tools`
3. Set `delay` to `200` (we can leave others the default value)
4. Open `ParrelSync -> Clones Manager`
5. Click `Add new clone`
6. After a while, there'll be a option called `Open in new Editor` click on it
7. In this new editor start the game and select `Server`
8. In the previous editor also start the game and select `Client`
9. View the client editor calculated ping

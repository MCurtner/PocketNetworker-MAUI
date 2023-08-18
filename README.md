# PocketNetworker

Port of existing iOS app to .NET MAUI. Enter IP address and netmask and it calculates the resulting broadcast, network, Cisco wildcard mask, and host range.

## Info

- CIDR notation (/24) or dotted decimals (255.255.255.0) for netmask input.
- The class of the network is determined by the first bits of the address.
- Address, Netmask, Wildcard, Network, Broadcast, Min/Max Hosts, and Hosts Availble outputs
- Binary representation of addresses
- Wildcard is the inverse of the netmask


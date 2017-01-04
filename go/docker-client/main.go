package main

import (
	"context"
	"encoding/json"
	"flag"
	"fmt"

	"github.com/docker/docker/api/types"
	"github.com/docker/docker/client"
)

var (
	host string
	ps   bool
	info bool
)

func init() {
	flag.StringVar(&host, "host", client.DefaultDockerHost, "host to reach")
	flag.BoolVar(&ps, "ps", false, "list containers (ps -a)")
	flag.BoolVar(&info, "info", false, "docker info")
}

func main() {
	flag.Parse()

	cli, err := client.NewClient(host, "", nil, nil)
	if err != nil {
		panic(err)
	}

	if ps {
		listContainers(cli)
		return
	}

	if info {
		displayInfo(cli)
		return
	}

	usage()
}

func dump(i interface{}) {
	jsonData, err := json.Marshal(i)
	if err != nil {
		panic(err)
	}
	fmt.Print(string(jsonData))
}

func listContainers(cli *client.Client) {
	containers, err := cli.ContainerList(context.Background(), types.ContainerListOptions{All: true})
	if err != nil {
		panic(err)
	}

	fmt.Print("{\"containers\":")
	dump(containers)
	fmt.Println("}")
}

func displayInfo(cli *client.Client) {
	info, err := cli.Info(context.Background())
	if err != nil {
		panic(err)
	}

	dump(info)
	fmt.Println("")
}

func usage() {
	fmt.Println("usage: docker-client [-host path] [-ps] [-info]")
}

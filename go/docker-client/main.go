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
	host    string
	compact bool
	ps      bool
	info    bool
	version bool
)

func init() {
	flag.StringVar(&host, "host", client.DefaultDockerHost, "host to reach")
	flag.BoolVar(&compact, "compact", false, "dump compact json")
	flag.BoolVar(&ps, "ps", false, "list containers (ps -a)")
	flag.BoolVar(&info, "info", false, "server info")
	flag.BoolVar(&version, "version", false, "server version")
}

func main() {
	flag.Parse()

	cli, err := client.NewClient(host, "", nil, nil)
	if err != nil {
		panic(err)
	}

	versionInfo, err2 := cli.ServerVersion(context.Background())
	if err2 != nil {
		panic(err2)
	}

	cli, err = client.NewClient(host, versionInfo.APIVersion, nil, nil)
	if err != nil {
		panic(err)
	}

	if version {
		serverVersion(cli)
		return
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
	var jsonData []byte
	var err error

	if compact {
		jsonData, err = json.Marshal(i)
	} else {
		jsonData, err = json.MarshalIndent(i, "", "    ")
	}

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

	fmt.Print("{ \"containers\": ")
	dump(containers)
	if compact {
		fmt.Print("}")
	} else {
		fmt.Println("}")
	}
}

func displayInfo(cli *client.Client) {
	info, err := cli.Info(context.Background())
	if err != nil {
		panic(err)
	}

	dump(info)
	if !compact {
		fmt.Println("")
	}
}

func serverVersion(cli *client.Client) {
	version, err := cli.ServerVersion(context.Background())
	if err != nil {
		panic(err)
	}

	dump(version)
	if !compact {
		fmt.Println("")
	}
}

func usage() {
	fmt.Println("usage: docker-client [-host path] [-compact] [-version] [-ps] [-info]")
}
